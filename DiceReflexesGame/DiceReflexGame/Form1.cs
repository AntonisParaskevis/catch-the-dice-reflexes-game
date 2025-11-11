using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiceReflexGame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // The score, the high score and the coordinates of the image are stored in these variables, respectively
        short score = 0, highscore = 0, coordinate_x, coordinate_y;

        // game_is_being_played: if it's set to "true", the game is running
        // game_can_start: if it's set to "true", all the conditions for the game to be able to start are fulfilled
        // clicked: if it's set to "false" and the game is running, the player can score points when they click on the image.
        // After clicking on the image, the variable will be set to "true", and the player won't be able to score more points until the image has moved
        public Boolean game_is_being_played, game_can_start, clicked = false;

        // The game time and the current dice roll are stored in these variables, respectively
        byte time = 60, roll;

        //Create and initialize an object of the Random class, in order to randomly generate new coordinates for the image
        Random random = new Random();

        // The game difficulty is stored in this variable. It will eventually be stored in the previous attempts file, along with the player's score, name and the time when the game was played
        string difficulty = "Medium";

        private void Form1_Load(object sender, EventArgs e)
        {
            //Check if the high score file exists
            if (!File.Exists(@"c:\Highscore_dicegame.txt"))
            {
                // If it doesn't exist, create it
                using (File.Create(@"c:\Highscore_dicegame.txt")) { }

                // Open the file, so the default highscore (0) can be entered
                TextWriter tsw = new StreamWriter(@"C:\Highscore_dicegame.txt", true);

                // Enter the default highscore, which is zero, into the file
                tsw.WriteLine(0);

                // Close the file, we don't need it open anymore
                tsw.Close();
            }
            else
            {
                // If the high score file exists, insert its contents (the high score) into the highscore variable
                highscore = short.Parse(File.ReadAllText(@"C:\Highscore_dicegame.txt"));

                // Update the GUI element that displays the high score
                label4.Text = "High Score: " + highscore;
            }

            // The location of the label5 element, which contains the text "Game Paused", is controlled through code, in order to make sure that it appears exactly in the center of the form
            label5.Left = (ClientSize.Width - label5.Width) / 2;
            label5.Top = (ClientSize.Height - label5.Height) / 2;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (game_is_being_played && clicked == false)
            {
                // Add the value of the dice to the score
                score = (short)(score + roll);

                // Update the GUI element that displays the score
                label1.Text = "Score: " + score;

                // Once the user clicks on the image, they can't score points by clicking on it again before it changes place
                clicked = true;

                if (score > highscore)
                {
                    // If the score surpasses the high score, make the current score the high score
                    label4.Text = "High Score: " + score;
                }
            }
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (game_can_start)
            {
                //Disable the name field, so that the player cannot change their name in mid-game
                textBox1.ReadOnly = true;

                // Start the game
                game_is_being_played = true;

                // Start the game timer
                timer1.Enabled = true;

                // The image starts changing places
                timer2.Enabled = true;

                // Disable the toolstrip menu items so the player won't be able to change the difficulty, view the previous attempts or start a new game while the game is running
                difficultyToolStripMenuItem.Enabled = false;
                gameToolStripMenuItem.Enabled = false;
                previousAttemptsToolStripMenuItem.Enabled = false;

                // Enable the pause button, so the player will be able to pause the game, if they need to
                button1.Enabled = true;

                // If the file containing the previous attempts doesn't exist, create it
                if (!File.Exists(@"c:\Previous_attempts_dicegame.txt"))
                {
                    using (File.Create(@"c:\Previous_attempts_dicegame.txt")) { }
                }
            }
            else
            {
                // If the user hasn't entered their name in the name field, display a message prompting them to do so
                MessageBox.Show("Enter your name in the field on the top side of the screen and try again.", "You didn't enter your name"); 
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (game_is_being_played)
            {
                // Every passing second, reduce the game time by 1
                time--;

                // Update the GUI item that displays the game time
                label2.Text = "Time: " + time;
            }
            if (time == 0)
            {
                // Re-enable the name field, so a different name can be entered, if desired
                textBox1.ReadOnly = false;

                // The game is over, so set game_is_being_played to "false"
                game_is_being_played = false;

                // The game time stops running
                timer1.Enabled = false;

                // The image stops changing places
                timer2.Enabled = false;

                // Disable the pause button, it's not needed since the game is not running
                button1.Enabled = false;

                // Display a message saying that the time is up, along with the player's score
                MessageBox.Show("Time is up!\n\nYour score: " + score, "Time is up!");

                // Open the file that contains the previous attempts, in order to record information about the game in it
                TextWriter tsw = new StreamWriter(@"C:\Previous_attempts_dicegame.txt", true);

                // Enter a line containing the player's name, score, current date and time, and game diffuculty into the file
                tsw.WriteLine(textBox1.Text + "    " + score + "    " + DateTime.Now + "    " + difficulty);

                // Close the file, we don't need it open anymore
                tsw.Close();

                if (score > highscore)
                {
                    // If the score is higher than the last high score, display a message telling the player that they've achieved a new high score, and show the high score in it
                    MessageBox.Show("Congratulations! You have set a new high score!\n\nNew high score: " + score, "New High Score!");

                    // Set the player's score equal to the highscore, this is the new high score now
                    highscore = score;

                    //update the GUI item that displays the high score
                    label4.Text = "High Score: " + highscore;

                    // Enter the new high score into the file in which it's stored, replacing the file's previous contents
                    File.WriteAllText(@"C:\Highscore_dicegame.txt", highscore.ToString());
                }

                // Reset the score and the time, so the game will be ready for the next playthrough
                score = 0;
                time = 60;

                // Update the GUI elements that display the score and the time
                label1.Text = "Score: 0";
                label2.Text = "Time: 60";

                // Re-enable the toolstrip menu items
                difficultyToolStripMenuItem.Enabled = true;
                gameToolStripMenuItem.Enabled = true;
                previousAttemptsToolStripMenuItem.Enabled = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Exit the game
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e) // pause/resume button
        {
            if (game_is_being_played) // If the game is running and the player pauses it
            {
                // Disable both timers, in order to freeze the game's functions
                timer1.Enabled = false;
                timer2.Enabled = false;

                // Change the word "Pause" in the pause/unpause button to "Resume", in order to make its function clear to the player
                button1.Text = "Resume";

                // Show the text "Game Paused" in the middle of the form, in order to notify the user that the game is paused
                label5.Visible = true;

                // The game isn't running, so set game_is_being_played to false
                game_is_being_played = false;
            }
            else // If the game is paused and the player unpauses it
            {
                // Re-enable both timers, in order to unfreeze the game's functions
                timer1.Enabled = true;
                timer2.Enabled = true;

                // Change the word "Resume" in the pause/unpause button to "Pause", in order to make its function clear to the player
                button1.Text = "Pause";

                // Hide the text "Game Paused" from the middle of the form, as it's unneeded and obstructive for the player
                label5.Visible = false;

                // The game is running once again, so set game_is_being_played to true
                game_is_being_played = true;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (game_is_being_played)
            {
                // Roll the digital dice, randomly generate a number from 1 to 6
                roll = (byte)random.Next(1, 7);

                // For each roll, show the corresponding face of the dice
                if (roll == 1) 
                {
                    pictureBox1.Image = Image.FromFile("zari1.png");
                }
                else if ( roll == 2)
                {
                    pictureBox1.Image = Image.FromFile("zari2.png");
                }
                else if (roll == 3)
                {
                    pictureBox1.Image = Image.FromFile("zari3.png");
                }
                else if (roll == 4)
                {
                    pictureBox1.Image = Image.FromFile("zari4.png");
                }
                else if (roll == 5)
                {
                    pictureBox1.Image = Image.FromFile("zari5.png");
                }
                else if (roll == 6)
                {
                    pictureBox1.Image = Image.FromFile("zari6.png");
                }

                // Randomly generate the new x coordinate of the image
                coordinate_x = (short)random.Next(0, ClientSize.Width - pictureBox1.Width);

                // Randomly generate the new y coordinate of the image
                coordinate_y = (short)random.Next(0, ClientSize.Height - pictureBox1.Height);

                // Place the image in the new coordinates
                pictureBox1.Location = new Point(coordinate_x, coordinate_y);

                // Re-enable the ability to score points when clicking on the image
                clicked = false;
            }
        }

        private void previousAttemptsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(@"c:\Previous_attempts_dicegame.txt"))
            {
                // If the previous attempts file exists, display all of its contents, the previous attempts, into a messagebox item
                string readText = File.ReadAllText(@"C:\Previous_attempts_dicegame.txt");
                MessageBox.Show(readText, "Previous Attempts");
            }
            else
            {
                // If the previous attempts file doesn't exist, display a message saying that there are no previous attempts
                MessageBox.Show("There are no previous attempts yet.", "No previous attempts yet");
            }
        }

        private void easyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check "Easy", uncheck the rest
            easyToolStripMenuItem.Checked = true;
            mediumToolStripMenuItem.Checked = false;
            hardToolStripMenuItem.Checked = false;

            // Make the image change places every 1,4 seconds
            timer2.Interval = 1400;

            // Change the difficulty string to "Easy" so that if a game with this difficulty enabled is played, this difficulty will be stored in the previous attempts file, along with the rest of the game's information
            difficulty = "Easy";
        }

        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check "Medium", uncheck the rest
            easyToolStripMenuItem.Checked = false;
            mediumToolStripMenuItem.Checked = true;
            hardToolStripMenuItem.Checked = false;

            // Make the image change places every 1 second
            timer2.Interval = 1000;

            // Change the difficulty string to "Medium" so that if a game with this difficulty enabled is played, this difficulty will be stored in the previous attempts file, along with the rest of the game's information
            difficulty = "Medium";
        }

        private void hardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check "Hard", uncheck the rest
            easyToolStripMenuItem.Checked = false;
            mediumToolStripMenuItem.Checked = false;
            hardToolStripMenuItem.Checked = true;

            // Make the image change places every 0,7 seconds
            timer2.Interval = 700;

            // Change the difficulty string to "Hard" so that if a game with this difficulty enabled is played, this difficulty will be stored in the previous attempts file, along with the rest of the game's information
            difficulty = "Hard";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                // If the name field contains no characters or contains only blank characters (spaces or tabs), the game cannot start
                game_can_start = false;
            }
            else
            {
                // Otherwise, the game can start
                game_can_start = true;
            }
        }
    }
}
