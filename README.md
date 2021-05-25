# COMS30400: Team *F* Games Project 2020/2021

The year is 2057. You are part of an anti-government group who steal and sell private intelligence.

Your mission is to infiltrate a military train station and steal backpacks containing government secrets without being caught and handed the death sentence. Fight your way past guards, avoid being detected, and most of all, get out alive.

## Flagship Technologies

- Pose Recognition
- Cooperative Multiplayer
- Proximity Voice Chat

## Play the game:

You can either clone this repo directly and open the folder `Freight/` following the instructions from the [running the game](#run-the-game) section

OR

Download the zip file from the [release](https://github.com/tomstark99/COMS30400/releases/tag/v1.0) page, extract it and open the `FreightZip/` folder in the same way

## Run the game:

1. Open your unity project folder (`Freight/` or `FreightZip/`) in unity `2019.4.18f1`
2. Go to Window ⇾ Package Manager ⇾ and install `cinemachine`
3. Go to `Assets/Scenes/` and open `MenuSceneNew`
4. Run the game in the unity editor by pressing play

## Build the game:

1. Create a folder called `build/` in the project root folder 
2. In unity go to file ⇾ build settings ⇾ and build the project to this folder
3. Once built remove `build/index.html`
4. Copy all files from `../html/`
5. Paste all files into `build/`
6. On command line navigate to the project root which contains the `build/` folder
```sh
$ cd /path/to/project/directory/
```
7. Run 
```sh
$ python -m http.server --directory build
```
8. Go to `localhost:8000` on a web browser of your choice (recommended is chrome)
