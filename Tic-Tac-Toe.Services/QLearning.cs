namespace Tic_Tac_Toe.Services
{
    public class QLearning
    {
        private double[,] QTable; // Tabela Q para armazenar os valores Q
        private Dictionary<int, Dictionary<int, double>> table;

        private double learningRate; // Taxa de aprendizado (alfa)
        private double discountFactor; // Fator de desconto (gama)
        private Random random;
            
        public QLearning(int numStates, int numActions, double learningRate, double discountFactor)
        { 
            Console.WriteLine(" numActions: " + numActions + " numStates: " + numStates);
            QTable = new double[10, 9];
           
            this.learningRate = learningRate;
            this.discountFactor = discountFactor;
            random = new Random();

            table = new Dictionary<int, Dictionary<int, double>>();
        }
        public void UpdateQValue(int state, int action, double qValue)
        {
            if (!table.ContainsKey(state))
            {
                table[state] = new Dictionary<int, double>();
            }

            table[state][action] = qValue;
        }

        public double GetQValue(int state, int action)
        {
            if (table.ContainsKey(state) && table[state].ContainsKey(action))
            {
                return table[state][action];
            }

            return 0.0; // Valor Q padrão (ou qualquer outro valor padrão desejado)
        }

        public double GetMaxQValue2(int state)
        {
            if (table.ContainsKey(state))
            {
                return table[state].Values.Max();
            }

            return 0.0; // Valor Q padrão (ou qualquer outro valor padrão desejado)
        }

        public void DisplayTable()
        {
            foreach (var state in table)
            {
                Console.WriteLine("Estado: " + state.Key);
                foreach (var action in state.Value)
                {
                    Console.WriteLine("  Ação: " + action.Key + " | Valor Q: " + action.Value);
                }
                Console.WriteLine();
            }
        }

        public int ChooseAction2(int state, int[][] gameState)
        {
            int numRows = gameState.Length;
            int numCols = gameState[0].Length;

            int numCells = numRows * numCols;
            List<int> validActions = new List<int>();

            // Encontra todas as ações válidas (células vazias)
            for (int i = 0; i < numCells; i++)
            {
                int row = i / numCols;
                int col = i % numCols;
                if (gameState[row][col] == 0)
                {
                    validActions.Add(i);
                }
            }

            // Converte o estado em uma matriz 2D correspondente
            int[][] stateArray = new int[numRows][];
            for (int i = 0; i < numRows; i++)
            {
                stateArray[i] = new int[numCols];
                for (int j = 0; j < numCols; j++)
                {
                    stateArray[i][j] = gameState[i][j];
                }
            }

            // Verifica se o estado atual não está presente na tabela de valores Q
            if (!table.ContainsKey(state))
            {
                // Adiciona o estado à tabela de valores Q com todas as nove ações e valores Q iniciais de 0.0
                Dictionary<int, double> initialActionValues = new Dictionary<int, double>();
                for (int i = 0; i < 9; i++)
                {
                    initialActionValues[i] = 0.0;
                }
                table[state] = initialActionValues;
            }


            Dictionary<int, double> actionValues = table[state];

            DisplayTable();

            double maxQValue = actionValues.Values.Max();
            List<int> maxQActions = new List<int>();
            foreach (var pair in actionValues)
            {
                if (pair.Value == maxQValue)
                {
                    maxQActions.Add(pair.Key);
                }
            }
            /*
            if (random.NextDouble() < explorationRate)
            {
                int chosenAction = validActions[random.Next(validActions.Count)];
                return chosenAction;
            }
            else
            {
                double maxQValue = actionValues.Values.Max();
                List<int> maxQActions = new List<int>();
                foreach (var pair in actionValues)
                {
                    if (pair.Value == maxQValue)
                    {
                        maxQActions.Add(pair.Key);
                    }
                }

                // Escolhe aleatoriamente uma ação com o valor Q máximo
                int chosenAction = maxQActions[random.Next(maxQActions.Count)];
                return chosenAction;
            }
            */
            return 0;
        }

        


        // Funções auxiliares para verificar o vencedor e o empate
        private bool CheckWin(int[][] gameState, int player)
        {
            int numRows = gameState.Length;
            int numCols = gameState[0].Length;

            // Verifica linhas e colunas
            for (int i = 0; i < numRows; i++)
            {
                if (gameState[i][0] == player && gameState[i][1] == player && gameState[i][2] == player)
                    return true;
                if (gameState[0][i] == player && gameState[1][i] == player && gameState[2][i] == player)
                    return true;
            }

            // Verifica diagonais
            if (gameState[0][0] == player && gameState[1][1] == player && gameState[2][2] == player)
                return true;
            if (gameState[2][0] == player && gameState[1][1] == player && gameState[0][2] == player)
                return true;

            return false;
        }

        private bool CheckDraw(int[][] gameState)
        {
            int numRows = gameState.Length;
            int numCols = gameState[0].Length;

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    if (gameState[i][j] == 0)
                        return false; // Ainda há células vazias, não é empate
                }
            }

            return true; // Todas as células estão preenchidas, é empate
        }

        private int GetState(int[][] gameState)
        {
            int numRows = gameState.Length;
            int numCols = gameState[0].Length;

            int state = 0;
            int factor = 1;

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    state += gameState[i][j] * factor;
                    factor *= 3; // 0: 0, 1: 1, -1: 2
                }
            }

            return state;
        }

        private List<int> GetValidActions(int[][] gameState)
        {
            int numRows = gameState.Length;
            int numCols = gameState[0].Length;

            int numCells = numRows * numCols;
            List<int> validActions = new List<int>();

            for (int i = 0; i < numCells; i++)
            {
                int row = i / numCols;
                int col = i % numCols;
                if (gameState[row][col] == 0)
                {
                    validActions.Add(i);
                }
            }

            return validActions;
        }

        public int ChooseAction(int state, int[][] gameState)
        {
            Console.WriteLine(state + " : state");
            
            
            
            int numActions = QTable.GetLength(1);
            List<int[]> bestActions = new List<int[]>();
            double bestQ = double.MinValue;

            int numRows = gameState.Length;
            int numCols = gameState[0].Length;

            int[,] convertedGameState = new int[numRows, numCols];
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    convertedGameState[i, j] = gameState[i][j]; // Copie os elementos da matriz int[][] para a matriz int[,]
                }
            }

            PrintGameState(convertedGameState);



            for (int action = 0; action < numActions; action++)
            {   
                // Verifica se a célula está vazia no estado atual do jogo
                int row = action / numCols;
                int col = action % numCols;

                if (row >= 0 && row < numRows && col >= 0 && col < numCols)
                {
                    if (convertedGameState[row, col] == 0)
                    {
                        double qValue = QTable[state, action];;
                        if (qValue > bestQ)
                        {
                            bestActions.Clear();
                            bestActions.Add(new[] { row, col });
                            bestQ = qValue;
                        }
                        else if (qValue == bestQ)
                        {
                            bestActions.Add(new[] { row, col });
                        }
                    }
                }
            }

            int[] chosenAction;
                

            // Implementação da estratégia epsilon-greedy para exploração e exploração
            double epsilon = 0.1; // Valor de epsilon (probabilidade de explorar)
            if (random.NextDouble() < epsilon)
            {
                chosenAction = bestActions[random.Next(bestActions.Count)];
            }
            else
            {
                chosenAction = bestActions[random.Next(bestActions.Count)];
            }

            int nextState = chosenAction[0] * numCols + chosenAction[1];
            // Chamada para atualizar os valores Q
            
            CheckGameOver(state, gameState, convertedGameState);
            Console.WriteLine("sinal test");
            //UpdateQValue(state, chosenAction, nextState, reward, numCols);
            RewardTerminalState(state, gameState);
            Console.WriteLine("sinal test1");

            return nextState;
        }

        public void PrintGameState(int[,] gameState)
        {
            int numRows = gameState.GetLength(0);
            int numCols = gameState.GetLength(1);

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    string symbol = " ";
                    if (gameState[i, j] == 1)
                        symbol = "X"; // Símbolo para o jogador humano
                    else if (gameState[i, j] == -1)
                        symbol = "O"; // Símbolo para a IA

                    Console.Write(symbol);
                    if (j < numCols - 1)
                        Console.Write(" | ");
                }

                Console.WriteLine();
                if (i < numRows - 1)
                    Console.WriteLine("---------");
            }
        }
            
        public double GetReward(int state, int[] action, int[,] gameState)
        {
            int numRows = gameState.GetLength(0);
            int numCols = gameState.GetLength(1);

            // Extrair a linha e a coluna da ação escolhida
            int row = action[0];
            int col = action[1];

            // Verificar se a ação resulta em uma vitória
            if (IsWinningMove(row, col, gameState))
            {
                // Retornar uma recompensa positiva para a vitória
                Console.WriteLine("Winning");
                return 1.0;
            }

            // Verificar se a ação resulta em um empate
            if (IsTieGame(gameState))
            {
                // Retornar uma recompensa neutra para o empate
                Console.WriteLine("Tie");
                return 0.5;
            }

            // Caso contrário, retornar uma recompensa negativa
            return -0.5;
        }

        public bool IsWinningMove(int row, int col, int[,] gameState)
        {
            // Implemente a lógica para verificar se a ação resulta em uma vitória
            // Verifique as linhas, colunas e diagonais que passam pela célula (row, col)
            // Se uma sequência de células contíguas do mesmo jogador for encontrada, é uma vitória

            // Exemplo:
            // Verifique a linha horizontal
            int player = gameState[row, col];
            bool isWinningRow = true;
            for (int c = 0; c < gameState.GetLength(1); c++)
            {
                if (gameState[row, c] != player)
                {
                    isWinningRow = false;
                    break;
                }
            }

            if (isWinningRow)
                return true;

            // Verifique a coluna vertical
            bool isWinningCol = true;
            for (int r = 0; r < gameState.GetLength(0); r++)
            {
                if (gameState[r, col] != player)
                {
                    isWinningCol = false;
                    break;
                }
            }

            if (isWinningCol)
                return true;

            // Verifique a diagonal principal
            bool isWinningDiagonal = true;
            for (int i = 0; i < gameState.GetLength(0); i++)
            {
                if (gameState[i, i] != player)
                {
                    isWinningDiagonal = false;
                    break;
                }
            }

            if (isWinningDiagonal)
                return true;

            // Verifique a diagonal secundária
            bool isWinningSecondaryDiagonal = true;
            for (int i = 0; i < gameState.GetLength(0); i++)
            {
                if (gameState[i, gameState.GetLength(0) - 1 - i] != player)
                {
                    isWinningSecondaryDiagonal = false;
                    break;
                }
            }
            return isWinningSecondaryDiagonal;
        }

        public bool IsTieGame(int[,] gameState)
        {
            // Implemente a lógica para verificar se o jogo terminou em empate
            // Verifique se todas as células estão preenchidas e não há mais movimentos possíveis
            for (int i = 0; i < gameState.GetLength(0); i++)
            {
                for (int j = 0; j < gameState.GetLength(1); j++)
                {
                    if (gameState[i, j] == 0)
                    {
                        return false; // Ainda existem células vazias, portanto o jogo não terminou em empate
                    }
                }
            }

            return true; // Todas as células estão preenchidas, portanto o jogo terminou em empate
        }


        public void UpdateQValue(int state, int[] action, int nextState, double reward, int numCols)
        {
            int actionIndex = action[0] * numCols + action[1];
            double maxQNextState = GetMaxQValue(nextState);
            double oldQ = QTable[state, actionIndex];
            double newQ = oldQ + learningRate * (reward + discountFactor * maxQNextState - oldQ);
            QTable[state, actionIndex] = newQ;
        }

        public void PrintQTableActions()
        {
            int numRows = QTable.GetLength(0);
            int numActions = QTable.GetLength(1);

            Console.Write("   |");
            for (int action = 0; action < numActions; action++)
            {
                Console.Write(" Ação " + action + " |");
            }
            Console.WriteLine();

            for (int state = 0; state < numRows; state++)
            {
                Console.Write(state.ToString("D2") + " |");
                for (int action = 0; action < numActions; action++)
                {
                    Console.Write("  " + QTable[state, action].ToString("F1") + "  |");
                }
                Console.WriteLine();
            }
        }


        private double GetMaxQValue(int state)
        {
            int numActions = QTable.GetLength(1);
            double maxQ = double.MinValue;

            for (int action = 0; action < numActions; action++)
            {
                double qValue = QTable[state, action];
                if (qValue > maxQ)
                {
                    maxQ = qValue;
                }
            }

            return maxQ;
        }

        public void RewardTerminalState(int state, int[][] gameState)
        {

            int numRows = gameState.Length;
            int numCols = gameState[0].Length;

            int[,] convertedGameState = new int[numRows, numCols];
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    convertedGameState[i, j] = gameState[i][j]; // Copie os elementos da matriz int[][] para a matriz int[,]
                }
            }
            double reward = GetTerminalStateReward(convertedGameState);
            for (int action = 0; action < QTable.GetLength(1); action++)
            {
                Console.WriteLine("state: " + state + " action: " + action);
                QTable[state, action] = reward;
                Console.WriteLine("state: adaadaada");
            }
        }

        private double GetTerminalStateReward(int[,] gameState)
        {
            if (IsWinningMove(0, 0, gameState) || IsWinningMove(0, 1, gameState) || IsWinningMove(0, 2, gameState) || IsWinningMove(1, 0, gameState) || IsWinningMove(1, 1, gameState) || IsWinningMove(1, 2, gameState) || IsWinningMove(2, 0, gameState) || IsWinningMove(2, 1, gameState) || IsWinningMove(2, 2, gameState))
            {
                return 1.0;
            }
            else if (IsTieGame(gameState))
            {
                return 0.5;
            }
            else
            {
                return -0.5;
            }
        }

        public bool CheckGameOver(int state, int[][] gameState, int[,] board)
        {
            // Verificar linhas
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] != 0 && board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2])
                {
                    Console.WriteLine("Jogo terminado! Resultado:");
                    PrintBoard(board);
                    RewardTerminalState(state, gameState);
                    return true;
                }
            }

            // Verificar colunas
            for (int j = 0; j < 3; j++)
            {
                if (board[0, j] != 0 && board[0, j] == board[1, j] && board[1, j] == board[2, j])
                {
                    Console.WriteLine("Jogo terminado! Resultado:");
                    PrintBoard(board);
                    RewardTerminalState(state, gameState);
                    return true;
                }
            }

            // Verificar diagonais
            if (board[0, 0] != 0 && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
            {
                Console.WriteLine("Jogo terminado! Resultado:");
                PrintBoard(board);
                RewardTerminalState(state, gameState);
                return true;
            }

            if (board[2, 0] != 0 && board[2, 0] == board[1, 1] && board[1, 1] == board[0, 2])
            {
                Console.WriteLine("Jogo terminado! Resultado:");
                PrintBoard(board);
                RewardTerminalState(state, gameState);
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
                PrintBoard(board);
                RewardTerminalState(state, gameState);
                return true;
            }

            return false;
        }

        public void PrintBoard(int[,] board)
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

    }
}
