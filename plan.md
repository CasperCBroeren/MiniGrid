# Mini grid
What is mini grid? Mini grid is a small educational game to teach about energy grids. A set of players can play through a year as a energy company.
The goal should be; who earns the most money and has the lowest carbon footprint

1. Each player will get to pick assets that produce or store energy for a certain amount of euro
1. Each player will get an amount of clients through the year
1. Weather conditions will change through the game year and seasons will impact the energy production
1. A game will be 365 days long, 1 year game time is 29 minutes real time; 1 hour is 20 seconds in real time
1. Penalties can be given when power needed is not supplied to the customers
1. Special chances can be taken by handling special cases like; emergency power, special clients or special assets to be bought
 
# Assets
## 🔥 Gas
- Price:       3M
- Carbon:      5
- Power:       5 kWh
- Reliability: 100%
- Ramp:        1 hour
- Fuel:        € 100 per day

## 🏭 Coal
- Price:       2M
- Carbon:      8
- Power:       3 kWh
- Reliability: 100%
- Ramp:        6 hours
- Fuel:        € 50 per day

## ☀️ Solar 
- Price:       2M
- Carbon:      1
- Power:       1 kWh
- Reliability: Bell curve during the year, 90% in summer, 10% in winter
- Ramp:        0
- Fuel:        Free

## 🌬️ Wind
- Price:       2M
- Carbon:      0.5
- Power:       1 kWh
- Reliability: 10% between 0 and 100% depending on the weather
- Ramp:        0
- Fuel:        Free

## 🔋 Battery
- Price:       2M
- Carbon:      0.5 
- Power:       4 kWh also cost 4kWh
- Reliability: 60% per day
- Ramp:        0
- Fuel:        Market or surplus energy (free)

## ☢️ Nuclear
- Price:       6M
- Carbon:      3
- Power:       10 kWh
- Reliability: 100%
- Ramp:        24 hours
- Fuel:        € 130 per day

## Game start
The game starts and the player gets € 6.000.000 million and its 1 january.
The game pauzes, the player can buy assets. The fuel cost of some are not included in the price.
The player can set an output per asset for some
The player gets 1000 clients, but can "buy" more per 1000 for € 100 .000
The each client will use between 2KW and 4KW perday.
The player can set a price per kWh for the clients, the default is € 0.10 per day
When played with more people the energy price will the vwap of the players.

## Weather
The weather is an important factor but also a bit shallow.
It's a ChatGPT suggested model in which we can get solar and wind 

                 Day of year
                      │
          ┌───────────┴───────────┐
          │                       │
     Solar model             Wind model
          │                       │
      sun position            seasonal mean
          │                       │
      clear sky              weather system
          │                       │
       clouds                random variation
          │                       │
          ▼                       ▼
    Solar 0..1                Wind m/s
          │                       │
          │                 turbine curve
          │                       │
          └──────────┬────────────┘
                     ▼
              Energy production