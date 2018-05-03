using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Media;
using Microsoft.Win32;

namespace game {
        class Player {

        public String name;

        private int startPosX;
        private int pointCounter;

        public const int playerSize = 30;
        const int maxSpeed = 7;
        const int acceleration = 1;
        const int gravity = 1;

        private double VelY;
        private double VelX;

        public double PosX;
        public double PosY;

        private bool canLeft = true;
        private bool canRight = true;
        private bool hitRoof = false;
        private bool onGround = false;
        private bool success;

        public bool movingLeft = false;
        public bool movingRight = false;
        public bool tryJump = false;
        
        

        public Player(String Name, int startPosX) {
            /* BASE VALUES FOR NEW PLAYER */
            this.name = Name;
            this.startPosX = startPosX;
            this.PosX = this.startPosX;
            this.PosY = Map.tileSize * 8;
            this.VelX = 0;
            this.VelY = 0;
            this.pointCounter = 0;
        }
        
        public void restart() { 
            /* RESTART GAME */       
            this.VelX = 0;
            this.VelY = 0;
            this.movingLeft = false;
            this.movingRight = false;
            this.tryJump = false;
            this.onGround = false;
            this.PosX = this.startPosX;
            this.PosY = 225;
            this.pointCounter = 0;
        }


        private bool getTileTypeByCoordinates(int x, int y) {
            /* RETURN CURRENT TILE'S TYPE AS BOOLEAN */
            return gameWindow.map_struct[y / Map.tileSize, x / Map.tileSize] == 1;
        }


        private void checkCollision() {

            try {
                /* ON GROUND */
                if (this.getTileTypeByCoordinates((int)this.PosX, (int)this.PosY + playerSize) ||
                   this.getTileTypeByCoordinates((int)this.PosX + playerSize / 2, (int)this.PosY + playerSize)) {

                    this.onGround = true;
                    this.VelY = 0;
                    this.PosY = this.PosY - this.PosY % Map.tileSize;
                } else {
                    this.onGround = false;
                }

                /* HIT ROOF */
                if (this.getTileTypeByCoordinates((int)this.PosX + 2, (int)this.PosY + 2) ||
                   this.getTileTypeByCoordinates((int)this.PosX - 2 + playerSize / 2, (int)this.PosY + 2)) {
                    hitRoof = true;
                    this.VelY *= -0.3;
                } else hitRoof = false;

                /* WALL ON THE RIGHT */
                if (this.getTileTypeByCoordinates((int)this.PosX + 2 + playerSize / 2 + (int)this.VelX, (int)this.PosY + 10) ||
                    this.getTileTypeByCoordinates((int)this.PosX + 2 + playerSize / 2 + (int)this.VelX, (int)this.PosY + playerSize - 2)) {

                    this.canRight = false;
                } else {
                    this.canRight = true;
                }

                /* WALL ON THE LEFT */
                if (this.getTileTypeByCoordinates((int)this.PosX - 2 + (int)this.VelX, (int)this.PosY + 10) ||
                    this.getTileTypeByCoordinates((int)this.PosX - 2 + (int)this.VelX, (int)this.PosY + playerSize - 2)) {
                    this.canLeft = false;
                } else {
                    this.canLeft = true;
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Player Collision");
            }

            try {

                /* HIT TRIGGER, ADD POINTS */
                if (gameWindow.map_struct[(int)(this.PosY / Map.tileSize), (int)((this.PosX + playerSize / 2) / Map.tileSize)] == 2) {

                    gameWindow.map_struct[(int)(this.PosY / Map.tileSize), (int)((this.PosX + playerSize / 2) / Map.tileSize)] = 0;
                    this.newCheckpoint();
                }
                else if (gameWindow.map_struct[(int)((this.PosY + playerSize) / Map.tileSize), (int)((this.PosX + playerSize / 2) / Map.tileSize)] == 2) {

                    gameWindow.map_struct[(int)((this.PosY + playerSize) / Map.tileSize), (int)((this.PosX + playerSize / 2) / Map.tileSize)] = 0;
                    this.newCheckpoint();
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Collision.Trigger");
            }

        }

        private void newCheckpoint() {

            try {

                Random rnd = new Random();
                success = false;

                int x;
                int y;

                this.pointCounter++;
                App.playSound("coin.wav");

                while (!success) {

                    x = rnd.Next(1, 39);
                    y = rnd.Next(1, 19);

                    if (gameWindow.map_struct[y, x] == 0) {
                        gameWindow.map_struct[y, x] = 2;
                        success = true;
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "newCheckpoint");
            }
        }

        /* PLAYER UPDATE */
        public bool update() {

            success = false;

            /* CHECK COLLISION */
            this.checkCollision();

            try {
                /* ON AIR */
                if (!this.onGround && !this.hitRoof) {
                    this.PosY += this.VelY;
                    this.VelY += gravity;
                } else if (!this.onGround && this.hitRoof) {
                    PosY = PosY + Map.tileSize - PosY % Map.tileSize;
                }
                  /* ON GROUND */
                  else {
                    this.onGround = true;
                    this.VelY = 0;
                }

                /* PRESS SPACE */
                if (this.tryJump) {
                    this.tryJump = false;
                    if (this.onGround) {
                        this.VelY -= 10;
                        this.PosY -= 5;
                        this.onGround = false;
                    }
                }

                /* MOVING LEFT */
                if (this.movingLeft && this.canLeft) {
                    this.VelX *= this.VelX >= 0 ? -1 : 1;
                    this.VelX -= -this.VelX >= maxSpeed ? 0 : acceleration;
                    this.PosX += this.VelX;
                }

                /* MOVING RIGHT */
                else if (this.movingRight && this.canRight) {
                    this.VelX *= this.VelX <= 0 ? -1 : 1;
                    this.VelX += this.VelX >= maxSpeed ? 0 : acceleration;
                    this.PosX += this.VelX;
                }

                /* NOT MOVING */
                else {
                    this.VelX = 0;
                }
            }   catch (Exception ex){
                MessageBox.Show(ex.Message, "Player.Update");
        }   return success;
    }
        
        
        public int getScore() {
            /* GET SCORE FOR PLAYER */
            return this.pointCounter;
        }

    }  
}
