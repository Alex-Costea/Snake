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

function getDisplayString(snakeBody, fruitPair)
{
    const display = initDisplay(maxX, maxY)
    i = 0;
    for(piece of snakeBody){
        if(i===snakeBody.length-1)
        {
            display[piece.y][piece.x] = '@'
        }
        else {
            display[piece.y][piece.x] = '%'
        }
        i++;
    }
    if(fruitPair !== null)
        display[fruitPair.y][fruitPair.x] = '*'

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
        nextDirection = nextDirections.length > 0 ? nextDirections.shift() : nextDirection
        if((lastDirection !== null) && modulo(lastDirection - nextDirection, 4) === 2)
            nextDirection = lastDirection
        const nextPair = getPositionAfterDirection(snakeBody[snakeBody.length - 1], nextDirection)
        const nextPairString = nextPair.asText()
        if(nextPairString === fruitPair.asText())
        {
            fruitPair = getFruitPair(freeSpaces)
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
        gamebody.innerHTML = getDisplayString(snakeBody, fruitPair)
    }, 50);
}