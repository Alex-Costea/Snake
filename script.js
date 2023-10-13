const maxX = 50;
const maxY = 30;

class Pair {
    constructor(x, y) {
      this.x = x;
      this.y = y;
    }
    asText()
    {
        return JSON.stringify(this)
    }
  }

function modulo(a,b)
{
    return (a+b) % b
}

function getPositionAfterDirection(pair, direction)
{
    switch(direction)
    {
        case 0: return new Pair(modulo(pair.x+1, maxX), modulo(pair.y, maxY)) //right
        case 1: return new Pair(modulo(pair.x, maxX), modulo(pair.y+1, maxY)) //down
        case 2: return new Pair(modulo(pair.x-1, maxX), modulo(pair.y, maxY)) //left
        case 3: return new Pair(modulo(pair.x, maxX), modulo(pair.y-1, maxY)) //up
        default: throw new Error("Direction unknown! " + direction)
    }
}

function getDirection(pair1, pair2)
{
    for(i=0;i<4;i++)
    {
        const newPair = getPositionAfterDirection(pair1, i);
        if(pair2.asText() === newPair.asText())
            return i;
    }
    throw new Error("Direction unknown!")
}

function initDisplay(maxX, maxY)
{
    let display = []
    for(i=0;i<maxY;i++)
    {
        let line = []
        for(j=0;j<maxX;j++)
        {
            line.push(" ")
        }
        display.push(line)
    }
    return display
}

function getFruitPair(freeSpaces)
{
    //run if this is efficient
    if(freeSpaces.size > maxX * maxY / 40)
    {
        //console.log("running v1")
        while(true)
        {
            const newFruitPair = new Pair(Math.floor(Math.random() * maxX), Math.floor(Math.random() * maxY))
            if(freeSpaces.has(newFruitPair.asText()))
            {
                return newFruitPair
            }
        }
    }
    //console.log("running v2")
    if(freeSpaces.size === 0)
        return null
    const freeSpacesEntries = Array.from(freeSpaces)
    const randomPick = Math.floor(Math.random() * freeSpaces.size)
    const pair = JSON.parse(freeSpacesEntries[randomPick])
    return new Pair(pair.x, pair.y)
}

function getBlock(previousDirection, nextDirection)
{
    const value = `${previousDirection}, ${nextDirection}`
    switch (value)
    {
        case '0, 0': return '&#9552;'
        case '2, 2': return '&#9552;'
        case '1, 1': return '&#9553;'
        case '3, 3': return '&#9553;'
        case '0, 1': return '&#9559;'
        case '3, 2': return '&#9559;'
        case '0, 3': return '&#9565;'
        case '1, 2': return '&#9565;'
        case '2, 1': return '&#9556;'
        case '3, 0': return '&#9556;'
        case '1, 0': return '&#9562;'
        case '2, 3': return '&#9562;'
        default: throw new Error(`Cannot get block element ${value}`)
    }
}

let toggle = 0
function getFruitCharacter()
{
    toggle = modulo(toggle+1, 20)
    return toggle >= 10 ? '&#9626;' : '&#9630;'
}

function getDisplayString(snakeBody, fruitPair)
{
    const display = initDisplay(maxX, maxY)
    for(let i=0;i<snakeBody.length;i++){
        const piece = snakeBody[i]
        if(i===snakeBody.length-1)
        {
            display[piece.y][piece.x] = '&#9608;'
        }
        else {
            const lastPiece = (i!==0) ? snakeBody[i-1] : null
            const nextPiece = snakeBody[i+1]
            const nextDirection = getDirection(piece, nextPiece)
            const previousDirection = lastPiece !== null ? getDirection(lastPiece,piece) : nextDirection
            display[piece.y][piece.x] = getBlock(previousDirection, nextDirection)
        }
    }
    if(fruitPair !== null)
        display[fruitPair.y][fruitPair.x] = getFruitCharacter()

    return display.map(x => x.join('')).join('\n')
}


function init()
{
    const snakeSize = 10;
    const direction = Math.floor(Math.random() * 4)
    const snakeOriginX = Math.floor(Math.random() * maxX)
    const snakeOriginY = Math.floor(Math.random() * maxY)
    const snakeBody = []

    let pair = new Pair(snakeOriginX, snakeOriginY)
    for(i=0;i<snakeSize;i++)
    {
        snakeBody.push(pair)
        pair = getPositionAfterDirection(pair, direction)
    }
    const snakeBodySet = new Set(snakeBody.map(x => x.asText()))

    const freeSpaces = new Set()
    for(i=0;i<maxX;i++)
    {
        for(j=0;j<maxY;j++)
        {
            const newPair = new Pair(i,j)
            if(!snakeBodySet.has(newPair.asText()))
                freeSpaces.add(newPair.asText())
        }
    }

    const gamebody = document.getElementById("gamebody")

    let fruitPair = getFruitPair(freeSpaces)
    gamebody.innerHTML = getDisplayString(snakeBody, fruitPair)

    let gameOver = false
    const nextDirections = [direction]
    window.addEventListener("keydown", (event) => {
        if (event.isComposing || event.keyCode === 229) {
          return;
        }
        if(nextDirections.length >= 2)
            return;
        if(event.keyCode === 37) // left
            nextDirections.push(2)
        if(event.keyCode === 38) // up
            nextDirections.push(3)
        if(event.keyCode === 39) // right
            nextDirections.push(0)
        if(event.keyCode === 40) // down
            nextDirections.push(1)
      });
    let nextDirection = null

    setInterval(() =>{
        if(gameOver)
            return
        const lastDirection = nextDirection
        if(nextDirections.length > 0)
        {
            nextDirection = nextDirections.shift()
            if(lastDirection !== null)
            {
                if(modulo(lastDirection - nextDirection, 4) === 2)
                {
                    if(nextDirections.length === 0)
                    {
                        nextDirection = lastDirection
                    }
                    else if(nextDirections.length === 1)
                    {
                        //reverse order
                        nextDirections.push(nextDirection)
                        nextDirection = nextDirections.shift()
                    }
                    else throw new Error("Too many control keys!")
                }
            }
        }
        const nextPair = getPositionAfterDirection(snakeBody[snakeBody.length - 1], nextDirection)
        const nextPairString = nextPair.asText()
        let foundSnack = false
        if((fruitPair!== null) && (nextPairString === fruitPair.asText()))
        {
            foundSnack = true
        }
        else{
            const firstPairOfSnakeBody = snakeBody.shift().asText()
            snakeBodySet.delete(firstPairOfSnakeBody)
            freeSpaces.add(firstPairOfSnakeBody)
        }
        if(snakeBodySet.has(nextPairString))
        {
            gameOver = true
        }
        snakeBody.push(nextPair)
        snakeBodySet.add(nextPairString)
        freeSpaces.delete(nextPairString)
        if(foundSnack)
        {
            fruitPair = getFruitPair(freeSpaces)
        }
        gamebody.innerHTML = getDisplayString(snakeBody, fruitPair)
    }, 50);
}