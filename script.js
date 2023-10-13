const maxX = 50;
const maxY = 30;
const snakeSize = 5;

class Pair {
    constructor(x, y) {
      this.x = x;
      this.y = y;
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

function getFruitPair(snakeBodySet)
{
    while(true)
    {
        const newFruitPair = new Pair(Math.floor(Math.random() * maxX), Math.floor(Math.random() * maxY))
        if(!snakeBodySet.has(JSON.stringify(newFruitPair)))
        {
            return newFruitPair
        }
    }
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
    display[fruitPair.y][fruitPair.x] = '*'

    return display.map(x => x.join('')).join('\n')
}


function init()
{
    const direction = Math.floor(Math.random() * 4)
    const snakeOriginX = Math.floor(Math.random() * maxX)
    const snakeOriginY = Math.floor(Math.random() * maxY)
    let pair = new Pair(snakeOriginX, snakeOriginY)

    let i = 0;
    const snakeBody = []
    for(i=0;i<snakeSize;i++)
    {
        snakeBody.push(pair)
        pair = getPositionAfterDirection(pair, direction)
    }
    const snakeBodySet = new Set(snakeBody.map(x => JSON.stringify(x)))

    const gamebody = document.getElementById("gamebody")
    

    let fruitPair = getFruitPair(snakeBodySet)
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
        nextDirection = nextDirections.length > 1 ? nextDirections.shift() : nextDirections[0]
        if(modulo(lastDirection - nextDirection, 4) === 2)
            nextDirection = lastDirection
        const nextPair = getPositionAfterDirection(snakeBody[snakeBody.length - 1], nextDirection)
        const nextPairString = JSON.stringify(nextPair)
        if(nextPairString === JSON.stringify(fruitPair))
        {
            fruitPair = getFruitPair(snakeBodySet)
        }
        else if(snakeBodySet.has(nextPairString))
        {
            gameOver = true
        }
        else
        {
            snakeBodySet.delete(JSON.stringify(snakeBody.shift()))
        }
        snakeBody.push(nextPair)
        snakeBodySet.add(nextPairString)
        gamebody.innerHTML = getDisplayString(snakeBody, fruitPair)
    }, 50);
}