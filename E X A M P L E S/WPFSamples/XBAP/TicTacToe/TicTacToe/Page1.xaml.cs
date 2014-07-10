using System;
using System.Windows;
using System.Windows.Media;

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : System.Windows.Controls.Page
    {
        int[,] occupied;
        int currentTurn; //0 - humam, 1-computer        
        int endGame;
        LinearGradientBrush nbrush;
        LinearGradientBrush obrush;

        public Page1()
        {
            InitializeComponent();

            nbrush = new LinearGradientBrush(Colors.White, Colors.PeachPuff, 90);
            obrush = new LinearGradientBrush(Colors.White, Colors.LightGray, 90);

            StartNewGame();
            currentTurn = 0;

        }

        void Click00(object sender, RoutedEventArgs e)
        {
            //if (currentTurn == 1)
            //    return;

            if (canPlace(currentTurn, 0, 0))
                doPlace(currentTurn, 0, 0);
        }

        void Click10(object sender, RoutedEventArgs e)
        {
            //if (currentTurn == 1)
            //    return;

            if (canPlace(currentTurn, 1, 0))
                doPlace(currentTurn, 1, 0);
        }

        void Click20(object sender, RoutedEventArgs e)
        {
            //if (currentTurn == 1)
            //    return;

            if (canPlace(currentTurn, 2, 0))
                doPlace(currentTurn, 2, 0);
        }

        void Click01(object sender, RoutedEventArgs e)
        {
            //if (currentTurn == 1)
             //   return;
            if (canPlace(currentTurn, 0, 1))
                doPlace(currentTurn, 0, 1);
        }

        void Click11(object sender, RoutedEventArgs e)
        {
            //if (currentTurn == 1)
             //   return;
            if (canPlace(currentTurn, 1, 1))
                doPlace(currentTurn, 1, 1);
        }

        void Click21(object sender, RoutedEventArgs e)
        {
            //if (currentTurn == 1)
            //    return;
            if (canPlace(currentTurn, 2, 1))
                doPlace(currentTurn, 2, 1);
        }

        void Click02(object sender, RoutedEventArgs e)
        {
            //if (currentTurn == 1)
            //    return;
            if (canPlace(currentTurn, 0, 2))
                doPlace(currentTurn, 0, 2);
        }

        void Click12(object sender, RoutedEventArgs e)
        {
            //if (currentTurn == 1)
            //    return;
            if (canPlace(currentTurn, 1, 2))
                doPlace(currentTurn, 1, 2);
        }

        void Click22(object sender, RoutedEventArgs e)
        {
            //if (currentTurn == 1)
            //    return;
            if (canPlace(currentTurn, 2, 2))
                doPlace(currentTurn, 2, 2);
        }

        void ClickNewGame(object sender, RoutedEventArgs e)
        {
            StartNewGame();
            currentTurn = 0;
        }

        void ClickNewGame2(object sender, RoutedEventArgs e)
        {
            StartNewGame();

            currentTurn = 1;

            MakeNextMove(occupied);

            //MakeNextMove
        }

        void StartNewGame()
        {
            currentTurn = 0;
            endGame = 0;

            occupied = new int[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                    occupied[i, j] = -1;

            }

            b00.Content = "";
            b10.Content = "";
            b20.Content = "";

            b01.Content = "";
            b11.Content = "";
            b21.Content = "";

            b02.Content = "";
            b12.Content = "";
            b22.Content = "";

            ResetBackground();
            
            MessageBlock.Text = "";
        }
        
        bool canPlace(int turn, int colum, int row)
        {
            if (endGame==1)
                return false;

            if (occupied[colum, row]==-1)
                return true;

            return false;
        }

        int doPlace(int turn, int colum, int row)
        {
            occupied[colum, row] = turn;

            ResetBackground();
            if ((colum == 0) && (row == 0))
            {
                if (turn == 0)
                    b00.Content = "O";
                else
                {
                    b00.Content = "X";
                    b00.Background = nbrush;
                }

            }
            else if ((colum == 1) && (row == 0))
            {
                if (turn == 0)
                    b10.Content = "O";
                else
                {
                    b10.Content = "X";
                    b10.Background = nbrush;
                }


            }
            else if ((colum == 2) && (row == 0))
            {
                if (turn == 0)
                    b20.Content = "O";
                else
                {
                    b20.Content = "X";
                    b20.Background = nbrush;

                }

            }
            else if ((colum == 0) && (row == 1))
            {
                if (turn == 0)
                    b01.Content = "O";
                else
                {
                    b01.Content = "X";
                    b01.Background = nbrush;
                }

            }
            else if ((colum == 1) && (row == 1))
            {
                if (turn == 0)
                    b11.Content = "O";
                else
                {
                    b11.Content = "X";
                    b11.Background = nbrush;
                }

            }
            else if ((colum == 2) && (row == 1))
            {
                if (turn == 0)
                    b21.Content = "O";
                else
                {
                    b21.Content = "X";
                    b21.Background = nbrush;

                }

            }
            else if ((colum == 0) && (row == 2))
            {
                if (turn == 0)
                    b02.Content = "O";
                else
                {
                    b02.Content = "X";
                    b02.Background = nbrush;

                }

            }
            else if ((colum == 1) && (row == 2))
            {
                if (turn == 0)
                    b12.Content = "O";
                else
                {
                    b12.Content = "X";
                    b12.Background = nbrush;
                }

            }
            else if ((colum == 2) && (row == 2))
            {
                if (turn == 0)
                    b22.Content = "O";
                else
                {
                    b22.Content = "X";
                    b22.Background = nbrush;
                }
            }


            if (checkEndGame(occupied,turn, colum, row) == 1)
            {
                if (turn == 1)
                    MessageBlock.Text = "Computer Wins";
                else
                    MessageBlock.Text = "Player Wins";
                endGame = 1;

            }
            else if (checkEndGame(occupied, turn, colum, row) == 2)
            {
                
                MessageBlock.Text = "Draw";
                endGame = 1;

            }

            if (endGame == 0)
            {
                if (turn == 0)
                {
                    currentTurn = 1;
                    MakeNextMove(occupied);
                }
                else
                    currentTurn = 0;
            }

            return 0;
        }
                
        int MakeNextMove(int[,] currentGame)
        {
            int move;
                        
            int allEmpty = 1;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (currentGame[i, j] != -1)
                        allEmpty = 0;

                }
            }
            

            if (allEmpty == 0)
            {

                int[] win_array = new int[9];
                int[] lose_array = new int[9];
                int[] draw_array = new int[9];

                int win_items = 0;
                int lose_items = 0;
                int draw_items = 0;

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {

                        if (currentGame[i, j] == -1) //if not occupied
                        {
                            int cnm = CheckNextMove(currentGame, 1, i, j);
                            if (cnm == 0)
                            {
                                move = i * 3 + j;
                                lose_array[lose_items] = move;
                                lose_items++;
                            }
                            else if (cnm == 1)
                            {
                                move = i * 3 + j;
                                draw_array[draw_items] = move;
                                draw_items++;

                            }
                            else if (cnm == 2)
                            {
                                move = i * 3 + j;
                                win_array[win_items] = move;
                                win_items++;

                            }
                        }

                    }
                }


                //Choose from among best choices
                int x;
                Random val = new Random();
                if (win_items > 0)
                {
                    x = val.Next() % win_items;
                    move = win_array[x];
                }
                else if (draw_items > 0)
                {
                    x = val.Next() % draw_items;
                    move = draw_array[x];

                }
                else if (lose_items > 0)
                {
                    x = val.Next() % lose_items;
                    move = lose_array[x];

                }
                else
                    move = -1;

            }
            else
            {
                //Computer 1st move is random
                
                Random val = new Random();
                move = val.Next() % 9;
                if (move == 5)
                {

                }
                else if ((move % 2) == 1)
                {
                    //give lesser chance to side squares
                    move = val.Next() % 9;

                }

            }
            
            //Make the actual move
            int column, row;
            column = move / 3;
            row = move % 3;
            doPlace(1, column, row);

            return move;

        }

        //returns 0 if this move can be definitely defeated by opponent eventually
        //returns 1 if this move can definitely draw with opponent eventually
        //returns 2 if this move can definitely win opponent eventually        
        int CheckNextMove(int[,] currentGame, int turn, int colum, int row)
        {            
            int[,] workGame = new int[3, 3];
            for (int i = 0; i < 3; i++)
            {

                for (int j = 0; j < 3; j++)
                {
                    workGame[i, j] = currentGame[i, j];
                }

            }
            workGame[colum, row] = turn;

            
            if (checkEndGame(workGame, turn, colum, row)==1)
            {
                return 2;

            }
            else if (checkEndGame(workGame, turn, colum, row) == 2)
            {
                return 1;

            }
            else //checkEndGame returns 0...game continues
            {
                int newturn;
                if (turn == 1)
                    newturn = 0;
                else
                    newturn = 1;

                int[] win_array = new int[9];
                int[] lose_array = new int[9];
                int[] draw_array = new int[9];

                int win_items = 0;
                int lose_items = 0;
                int draw_items = 0;

                int move = -1;                
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (workGame[i, j] == -1) //if not occupied
                        {
                            int cnm = CheckNextMove(workGame, newturn, i, j);

                            if (cnm == 0)
                            {
                                move = i * 3 + j;
                                lose_array[lose_items] = move;
                                lose_items++;
                            }
                            else if (cnm == 1)
                            {
                                move = i * 3 + j;
                                draw_array[draw_items] = move;
                                draw_items++;

                            }
                            else if (cnm == 2)
                            {

                                //optimization here
                                //a possible counter move by opponent that results in their sure-win means this move is a sure lose
                                return 0;

                                //move = i * 3 + j;
                                //win_array[win_items] = move;
                                //win_items++;

                            }
                        }
                    }
                }               
                
                                
                //if taking this moves results in a possible opponent counter move that grant them a sure-win
                //then this move is a sure-lose move
                if (win_items > 0)
                {
                    return 0;
                }
                //if taking this moves results in opponent best play is to draw                 
                //then this move can at least guarantee a draw for us
                else if (draw_items > 0)                
                {
                    return 1;

                }
                //if all possible opponent moves result in opponent losing
                //then this move is a sure win
                else if (lose_items > 0)
                {
                    return 2;                      
                }
               
                
                return 1;

            }//...if game continues

        }

        //return 1 for win
        //return 0 for non end game
        //reutrn 2 for draw
        int checkEndGame(int[,] currentGame, int turn, int colum, int row)
        {
                        
            int [,] workGame = new int[3,3];
            for (int i = 0; i < 3; i++)
            {

                for (int j = 0; j < 3; j++)
                {                    
                    workGame[i,j]=currentGame[i,j];
                }

            }
            workGame[colum, row] = turn;

            if (((workGame[0, 0] == turn) && (workGame[0, 1] == turn) && (workGame[0, 2] == turn)) ||
                ((workGame[1, 0] == turn) && (workGame[1, 1] == turn) && (workGame[1, 2] == turn)) ||
                ((workGame[2, 0] == turn) && (workGame[2, 1] == turn) && (workGame[2, 2] == turn)) ||

                ((workGame[0, 0] == turn) && (workGame[1, 0] == turn) && (workGame[2, 0] == turn)) ||
                ((workGame[0, 1] == turn) && (workGame[1, 1] == turn) && (workGame[2, 1] == turn)) ||
                ((workGame[0, 2] == turn) && (workGame[1, 2] == turn) && (workGame[2, 2] == turn)) ||

                ((workGame[0, 0] == turn) && (workGame[1, 1] == turn) && (workGame[2, 2] == turn)) ||
                ((workGame[0, 2] == turn) && (workGame[1, 1] == turn) && (workGame[2, 0] == turn)))
            {
                return 1;  //this move makes an end game with win for the current turn palyer
            }


            int allOccupied = 1;            
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (workGame[i, j] == -1)
                        allOccupied = 0;

                }

            }

            if (allOccupied == 1)
                return 2;

            return 0;

        }

        void ResetBackground()
        {

            b00.Background = obrush;
            b10.Background = obrush;
            b20.Background = obrush;

            b01.Background = obrush;
            b11.Background = obrush;
            b21.Background = obrush;

            b02.Background = obrush;
            b12.Background = obrush;
            b22.Background = obrush;
        }
    }
}