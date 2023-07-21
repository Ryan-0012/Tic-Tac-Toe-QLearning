using Tic_Tac_Toe.Models;
using Tic_Tac_Toe.Services;

namespace Tic_Tac_Toe.Game
{
    public class GameLogic
    {

        private const int BoardSize = 3; // Tamanho do tabuleiro
        private const int PlayerX = 1; // Valor do jogador X no tabuleiro
        private const int PlayerO = -1; // Valor do jogador O no tabuleiro
        private const int EmptyCell = 0; // Valor para célula vazia

        private static int[,] board = new int[3, 3]; // Matriz do jogo da velha

        private readonly QLearning2 _qLearning;

        
        decimal epsilon = 0.0m;
        decimal maxQ;

        int contO = 0;

        int[] actionsO = new int[9];
        int[] statesO = new int[9];
        int[] newStatesO = new int[9];
        decimal[] rewardsO = new decimal[9];
        int playsO = 0;
        decimal rewardO;


        int contX = 0;

        int[] actionsX = new int[9];
        int[] statesX = new int[9];
        int[] newStatesX = new int[9];
        decimal[] rewardsX = new decimal[9];
        int playsX = 0;
        decimal rewardX;

        static int actionO = 0;
        static int stateO = 0;

        int actionX = 0;
        int stateX = 0;

        public GameLogic(QLearning2 qLearning)
        {
            _qLearning = qLearning;

            if (IsGameOver())
                InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    board[i, j] = 0; // Preenche todas as células com 0 (vazio)
                }
            }
        }

        private void ApplyAction(int action, int currentPlayer)
        {
            int row = (action - 1) / BoardSize;
            int col = (action - 1) % BoardSize;
            board[row, col] = currentPlayer;
        }

        private List<int> GetValidActions()
        {
            List<int> validActions = new List<int>();

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (board[i, j] == EmptyCell)
                    {
                        validActions.Add(i * BoardSize + j + 1);
                    }
                }
            }

            return validActions;
        }

        // Obtém a recompensa para o jogador atual
        private decimal GetReward(int currentPlayer)
        {
            int countCell = 0;

            // Percorre cada elemento da matriz
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == currentPlayer)
                    {
                        countCell++; // Incrementa a contagem se o valor for igual ao valor desejado
                    }
                }
            }

            if (IsWinner(currentPlayer))
            {
                if (countCell == 5)
                    return 1.0m; // Jogador atual venceu
                if (countCell == 4)
                    return 1.5m; // Jogador atual venceu
                if (countCell == 3)
                    return 2.0m; // Jogador atual venceu

                return 1.0m;
            }
            else if (IsWinner(currentPlayer * -1))
            {
                if (countCell == 5)
                    return -1.0m; // Jogador atual venceu
                if (countCell == 4)
                    return -1.5m; // Jogador atual venceu
                if (countCell == 3)
                    return -2.0m; // Jogador atual venceu

                return -1.0m;
            }
            else if (IsDraw())
            {
                return 0.0m; // Empate
            }
            else
            {
                return 0.0m; // Jogo ainda não acabou
            }
        }

        // Verifica se o jogo acabou
        private bool IsGameOver()
        {
            return IsWinner(PlayerX) || IsWinner(PlayerO) || IsDraw();
        }

        // Verifica se um jogador venceu
        private bool IsWinner(int player)
        {
            // Verifica linhas
            for (int i = 0; i < BoardSize; i++)
            {
                if (board[i, 0] == player && board[i, 1] == player && board[i, 2] == player)
                {
                    return true;
                }
            }

            // Verifica colunas
            for (int j = 0; j < BoardSize; j++)
            {
                if (board[0, j] == player && board[1, j] == player && board[2, j] == player)
                {
                    return true;
                }
            }

            // Verifica diagonais
            if (board[0, 0] == player && board[1, 1] == player && board[2, 2] == player)
            {
                return true;
            }

            if (board[0, 2] == player && board[1, 1] == player && board[2, 0] == player)
            {
                return true;
            }

            return false;
        }

        // Verifica se o jogo terminou em empate
        private bool IsDraw()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (board[i, j] == EmptyCell)
                    {
                        return false; // Ainda há células vazias, o jogo não terminou em empate
                    }
                }
            }

            return true; // Todas as células estão preenchidas, jogo terminou em empate
        }

        public int MakeMove(int player, int state)
        {
            if (player == PlayerX)
            {
                SetGameStateFromNumber(state);
                PrintBoard();
                List<int> validActions = GetValidActions();
                actionO = _qLearning.ChooseAction(state, player * -1, epsilon, false, validActions);
                return actionO;
            }
            /*
            if (player == PlayerX)
            {
                // Jogada do PlayerX
                PrintBoard();

                List<int> validActionsX = GetValidActions();

                if (!validActionsX.Contains(position))
                {
                    Console.WriteLine("Essa célula já está ocupada. Escolha outra célula.");
                    PrintBoard();
                    return player * 100;
                }
                stateX = GetGameStateAsNumber();
                actionX = position;

                ApplyAction(position, player); // Marca a célula com o símbolo do jogador (1 para jogador humano, -1 para IA)//
                statesX[playsX] = stateX;
                actionsX[playsX] = actionX;


                int newStateX = GetGameStateAsNumber();

                newStatesX[playsX] = newStateX;
                rewardX = GetReward(player);

                rewardsX[playsX] = rewardX;

                decimal maxQ = _qLearning.GetMaxQ(newStateX, player);
                _qLearning.UpdateQValue(stateX, actionX, rewardX, maxQ, player);

                playsX++;
                contX++;

                if (IsGameOver())
                {
                    decimal maxQO = _qLearning.GetMaxQ(stateX, -1);
                    rewardO = GetReward(-1);
                    Console.WriteLine("State: " + stateO + " Action: " + actionO + " Reward: " + rewardO + " MaxQ: " + maxQ);
                    _qLearning.UpdateQValue(stateO, actionO, rewardO, maxQO, -1);

                    Console.WriteLine("test gameover");
                    if (rewardX > 0)
                        for (int i = 0; i < contX; i++)
                        {
                            maxQ = _qLearning.GetMaxQ(newStatesX[i], 1);
                            _qLearning.UpdateQValue(statesX[i], actionsX[i], rewardX, maxQ, 1);
                        }
                    PrintBoard();
                    return player * 200;

                }

                // Jogada do PlayerO
                contO++;
                stateO = GetGameStateAsNumber();
                List<int> validActionsO = GetValidActions();
                actionO = _qLearning.ChooseAction(stateO, -1, epsilon, false, validActionsO);

                statesO[playsO] = stateO;
                actionsO[playsO] = actionO;

                ApplyAction(actionO, -1);
                int newStateO = GetGameStateAsNumber();

                newStatesO[playsO] = newStateO;
                rewardO = GetReward(-1);

                rewardsO[playsO] = rewardO;

                maxQ = _qLearning.GetMaxQ(newStateO, -1);
                Console.WriteLine("State: " + stateO + " Action: " + actionO + " Reward: " + rewardO + " MaxQ: " + maxQ);
                _qLearning.UpdateQValue(stateO, actionO, rewardO, maxQ, -1);
                Console.WriteLine("O computador escolheu a posição " + actionO);
                playsO++;

                if (IsGameOver())
                {
                    rewardO = GetReward(-1);

                    if (rewardO > 0)
                        for (int i = 0; i < contO; i++)
                        {
                            maxQ = _qLearning.GetMaxQ(newStatesO[i], -1);
                            _qLearning.UpdateQValue(statesO[i], actionsO[i], rewardO, maxQ, -1);
                        }
                    PrintBoard();
                    return PlayerO * 200;
                }
                PrintBoard();
                return actionO;
            }
            */

            return player * 200;
          
        }

        public void SetGameStateFromNumber(int gameState)
        {
            int factor = 1;

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    int cellValue = (gameState / factor) % 3;
                    board[i, j] = GetCellFromValue(cellValue);
                    factor *= 3;
                }
            }
        }

        private int GetCellFromValue(int cellValue)
        {
            if (cellValue == 0)
            {
                return EmptyCell;
            }
            else if (cellValue == 1)
            {
                return PlayerX;
            }
            else if (cellValue == 2)
            {
                return PlayerO;
            }
            else
            {
                throw new Exception("Valor de célula inválido: " + cellValue);
            }
        }


        public void PrintBoard()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    string symbol = " ";
                    if (board[i, j] == 1)
                        symbol = "X"; // Símbolo para o jogador humano
                    else if (board[i, j] == -1)
                        symbol = "O"; // Símbolo para a IA

                    Console.Write(symbol);
                    if (j < 2)
                        Console.Write(" | ");
                }

                Console.WriteLine();
                if (i < 2)
                    Console.WriteLine("---------");
            }

        }


        public int[][] GetGameState()
        {
            int[][] gameState = new int[3][];
            for (int i = 0; i < 3; i++)
            {
                gameState[i] = new int[3];
                for (int j = 0; j < 3; j++)
                {
                    gameState[i][j] = board[i, j];
                }
            }
            return gameState;
        }
        /*
        public bool CheckGameOver()
        {
            // Verificar linhas
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] != 0 && board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2])
                {
                    Console.WriteLine("Jogo terminado! Resultado:");
                    PrintBoard();
                    _qLearning.RewardTerminalState(GetGameStateAsNumber(), GetGameState());
                    ResetBoard();
                    return true;
                }
            }

            // Verificar colunas
            for (int j = 0; j < 3; j++)
            {
                if (board[0, j] != 0 && board[0, j] == board[1, j] && board[1, j] == board[2, j])
                {
                    Console.WriteLine("Jogo terminado! Resultado:");
                    PrintBoard();
                    _qLearning.RewardTerminalState(GetGameStateAsNumber(), GetGameState());
                    ResetBoard();
                    return true;
                }
            }

            // Verificar diagonais
            if (board[0, 0] != 0 && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
            {
                Console.WriteLine("Jogo terminado! Resultado:");
                PrintBoard();
                _qLearning.RewardTerminalState(GetGameStateAsNumber(), GetGameState());
                ResetBoard();
                return true;
            }

            if (board[2, 0] != 0 && board[2, 0] == board[1, 1] && board[1, 1] == board[0, 2])
            {
                Console.WriteLine("Jogo terminado! Resultado:");
                PrintBoard();
                _qLearning.RewardTerminalState(GetGameStateAsNumber(), GetGameState());
                ResetBoard();
                return true;
            }

            // Verificar empate
            bool isFull = true;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == 0)
                    {
                        isFull = false;
                        break;
                    }
                }
                if (!isFull)
                    break;
            }

            if (isFull)
            {
                Console.WriteLine("Jogo terminado! Resultado:");
                PrintBoard();
                _qLearning.RewardTerminalState(GetGameStateAsNumber(), GetGameState());
                ResetBoard();
                return true;
            }

            return false;
        }
        */
        private void ResetBoard()
        {
            // Reset all cells to 0 (empty)
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    board[i, j] = 0;
                }
            }
        }


        public int GetGameStateAsNumber()
        {
            int gameState = 0;
            int factor = 1;

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    int cellValue = GetCellValue(board[i, j]);
                    gameState += cellValue * factor;
                    factor *= 3;
                }
            }

            return gameState;
        }

        private int GetCellValue(int cell)
        {
            if (cell == 0)
            {
                return 0; // Célula vazia
            }
            else if (cell == 1)
            {
                return 1; // Jogador humano
            }
            else if (cell == -1)
            {
                return 2; // IA
            }
            else
            {
                throw new Exception("Valor de célula inválido: " + cell);
            }
        }


        public int CountEmptyCells()
        {
            int count = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == 0)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

    }
}