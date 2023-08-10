![banner](https://www.yull.ca/provenceECS/images/banner-01.png)

## Links:
[Technical Write Up](https://www.yull.ca/provenceECS/)

## Overview: 

**Provence ECS** is a C# **entity component system** framework. Provence was created in, and initially designed for, the Unity game engine. Many steps have been taken to decouple Provence from Unity in-order to create a simple base ECS architecture for all needs.

## The Basics:

In an Entity Component System architecture, ECS from here on out, we break apart the traditional OOP structure into pieces to allow for incredibly varied behaviour. Instead of creating an class for each type of object you want to make, like a car or a bike, we start with an **entity**. An entity is solely a unique identifier.

We use components in place of an objects fields. Components are only data, they don't contain any methods. We assign components to entities and through the combination of different components we can create unique objects.

