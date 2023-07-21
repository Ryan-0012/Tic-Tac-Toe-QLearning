using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Tic_Tac_Toe.Services
{
    public class QLearning2
    {
        // Constantes para o jogo da velha
        private const int BoardSize = 3; // Tamanho do tabuleiro
        private const int NumActions = 9; // Número total de ações possíveis
        private const int PlayerX = 1; // Valor do jogador X no tabuleiro
        private const int PlayerO = -1; // Valor do jogador O no tabuleiro
        private const int EmptyCell = 0; // Valor para célula vazia
        private const decimal LearningRate = 0.3m; // Taxa de aprendizado do algoritmo Q-Learning
        private const decimal DiscountFactor = 0.6m; // Fator de desconto do algoritmo Q-Learning

        private Dictionary<int, Dictionary<int, decimal>> tableX; // Tabela de valores Q para o jogador X
        private Dictionary<int, Dictionary<int, decimal>> tableO; // Tabela de valores Q para o jogador O
        public static Dictionary<int, Dictionary<int, decimal>> tableXmemory = new Dictionary<int, Dictionary<int, decimal>>();
        public static Dictionary<int, Dictionary<int, decimal>> tableOmemory = new Dictionary<int, Dictionary<int, decimal>>();
        private static Dictionary<int, Dictionary<int, double>> tableOtest = new Dictionary<int, Dictionary<int, double>>();

        private int[,] board; // Representação do tabuleiro
        private int[,] tempBoard;

        private Random random;

        public QLearning2()
        {
            tableX = new Dictionary<int, Dictionary<int, decimal>>();
            tableO = new Dictionary<int, Dictionary<int, decimal>>();

            board = new int[BoardSize, BoardSize];
            InitializeBoard();
            InitializeTables();

            if (tableXmemory.Count == 0 && tableOmemory.Count == 0)
            {
                InitializeTableMemory();
            }
            

            random = new Random();
        }

        // Inicializa o tabuleiro com células vazias
        private void InitializeBoard()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    board[i, j] = EmptyCell;
                }
            }
        }

        private void InitializeTables()
        {
            tableX.Clear();
            tableO.Clear();

            for (int i = 0; i < Math.Pow(3, NumActions); i++)
            {
                tableX[i] = new Dictionary<int, decimal>();
                tableO[i] = new Dictionary<int, decimal>();

                for (int j = 1; j <= NumActions; j++)
                {
                    tableX[i][j] = 0.0m;
                    tableO[i][j] = 0.0m;
                }
            }
        }

        private void InitializeTableMemory()
        {
            tableXmemory.Clear();
            tableOmemory.Clear();
            for (int i = 0; i < Math.Pow(3, NumActions); i++)
            {
                tableXmemory[i] = new Dictionary<int, decimal>();
                tableOmemory[i] = new Dictionary<int, decimal>();

                for (int j = 1; j <= NumActions; j++)
                {
                    tableXmemory[i][j] = 0.0m;
                    tableOmemory[i][j] = 0.0m;
                }
            }
        }

        // Treina o agente Q-Learning jogando vários episódios do jogo
        public void Train(int numEpisodes, decimal epsilon)
        {
            for (int episode = 0; episode < numEpisodes; episode++)
            {
                int contO = 0;

                int[] actionsO = new int[9];
                int[] statesO = new int[9];
                int[] newStatesO = new int[9];
                int playsO = 0;
                decimal rewardO;


                int contX = 0;

                int[] actionsX = new int[9];
                int[] statesX = new int[9];
                int[] newStatesX = new int[9];
                int playsX = 0;
                decimal rewardX;

                actionsX = new int[9];
                statesX = new int[9];
                newStatesX = new int[9];

                actionsO = new int[9];
                statesO = new int[9];
                newStatesO = new int[9];

                int state = GetGameStateAsNumber(); // Obtém o estado atual do tabuleiro
                int currentPlayer = PlayerX; // Define o jogador atual como o jogador X

                int stateO;

                int stateX;
                

                while (!IsGameOver())
                {
                    if (currentPlayer == PlayerX)
                    {
                        if (contX > 0)
                            newStatesX[contX - 1] = GetGameStateAsNumber();
                        stateX = GetGameStateAsNumber();
                        contX++;
                        statesX[playsX] = stateX;
                        decimal maxQValidActions = GetMaxQValidActions(stateX, PlayerX);
                        // Mecanismo de punição de celulas anteriores 
                        
                        if (maxQValidActions < -0.8m)
                        {
                            decimal maxQValid = GetMaxQValidActions(statesX[contX - 1], PlayerX);
                            UpdateQValue(statesX[contX - 2], actionsX[contX - 2], -1.5m, maxQValid, PlayerX);
                        }
                        
                    }

                    if (currentPlayer == PlayerO)
                    {
                        stateO = GetGameStateAsNumber();
                        if (contO > 0)
                            newStatesO[contO - 1] = GetGameStateAsNumber();
                        contO++;
                        statesO[playsO] = stateO;
                        decimal maxQValidActions = GetMaxQValidActions(stateO, PlayerO);
                        // Mecanismo de punição de celulas anteriores 
                        
                        if (maxQValidActions < -0.8m)
                        {
                            decimal maxQValid = GetMaxQValidActions(statesO[contO - 1], PlayerO);
                            UpdateQValue(statesO[contO - 2], actionsO[contO - 2], -1.5m, maxQValid, PlayerO);
                        }
                        
                    }

                    List<int> validActions = GetValidActions(); // Obter as células disponíveis
                    List<int> possibleNewStates = new List<int>();

                    int action = ChooseAction(state, currentPlayer, epsilon, true, validActions); // Escolhe a ação a ser tomada
                    int newState = GetGameStateAsNumber(); // Obtém o novo estado do tabuleiro

                    ApplyAction(action, currentPlayer); // Aplica a ação no tabuleiro

                    validActions = GetValidActions();
                    tempBoard = board.Clone() as int[,];

                    foreach (int actionSim in validActions)
                    {
                        board = tempBoard.Clone() as int[,];
                        ApplyAction(actionSim, currentPlayer * -1);

                        // Obtém o número do estado após a simulação
                        int newStateSim = GetGameStateAsNumber();
                        possibleNewStates.Add(newStateSim);

                        decimal rewardSim = GetReward(currentPlayer); // Obtém a recompensa para o jogador atual
                        decimal maxQSim = GetMaxQ(newStateSim, currentPlayer); // Obtém o valor máximo de Q para o novo estado

                        if (rewardSim > 0 || rewardSim < 0)
                            UpdateQValue(newState, action, rewardSim, maxQSim, currentPlayer);
                    }
                    board = tempBoard.Clone() as int[,];

                    if (currentPlayer == PlayerX)
                    {
                        actionsX[playsX] = action;
                        int newStateX = GetGameStateAsNumber();
                        
                        if (playsX > 0)
                        {
                            rewardX = GetReward(PlayerX);
                            decimal maxQX = GetMaxQ(newState, currentPlayer);
                            if (rewardX > 0 || rewardX < 0)
                                UpdateQValue(statesX[playsX - 1], actionsX[playsX - 1], rewardX, maxQX, currentPlayer);
                        }
                       
                        playsX++;
                    }

                    if (currentPlayer == PlayerO)
                    {
                        actionsO[playsO] = action;
                        int newStateO = GetGameStateAsNumber();
                        

                        if (playsO > 0)
                        {
                            rewardO = GetReward(PlayerO);
                            decimal maxQO = GetMaxQ(newState, currentPlayer);
                            if (rewardO > 0 || rewardO < 0)
                                UpdateQValue(statesO[playsO - 1], actionsO[playsO - 1], rewardO, maxQO, currentPlayer);
                        }
                        
                        playsO++;
                    }


                    decimal reward = GetReward(currentPlayer); // Obtém a recompensa para o jogador atual
                    decimal maxQ = GetMaxQ(newState, currentPlayer); // Obtém o valor máximo de Q para o novo estado

                    int previusState = (currentPlayer == PlayerX) ? statesX[playsX - 1] : statesO[playsO - 1];
                    int previusAction = (currentPlayer == PlayerX) ? actionsX[playsX - 1] : actionsO[playsO - 1];
                    if (reward > 0 || reward < 0)
                        UpdateQValue(previusState, previusAction, reward, maxQ, currentPlayer);
                    //Atualiza o valor Q do estado atual e ação tomada
                     UpdateQValue(state, action, reward, maxQ, currentPlayer);

                    state = newState; // Atualiza o estado atual
                    currentPlayer *= -1; // Alterna o jogador atual
                }
                
                rewardO = GetReward(PlayerO);
                rewardX = GetReward(PlayerX);

                /*
                if (rewardX > 0)
                    for (int i = 1; i < contX; i++)
                    {
                        decimal maxQ = GetMaxQ(newStatesX[i], PlayerX);
                        UpdateQValue(statesX[i], actionsX[i], rewardX, maxQ, 1);
                    }
               
                if (rewardO > 0)
                    for (int i = 0; i < contO; i++)
                    {
                        decimal maxQ = GetMaxQ(newStatesO[i], PlayerO);
                        UpdateQValue(statesO[i], actionsO[i], rewardO, maxQ, -1);
                    }
                */
                
                InitializeBoard(); // Reinicia o tabuleiro para o próximo episódio
            }

        }

        // Escolhe a melhor ação para um determinado estado e jogador
        public int ChooseAction(int state, int currentPlayer, decimal epsilon, bool train, List<int> validActions)
        {
            if (train)
            {
                Dictionary<int, decimal> currentTable = (currentPlayer == PlayerX) ? tableXmemory[state] : tableOmemory[state];
                int action = -1;
                decimal maxQ = decimal.MinValue;

                double doubleValue = random.NextDouble();
                decimal randomValue = (decimal)doubleValue;

                // Percorre a tabela de valores Q para encontrar a ação com o maior valor Q
                if (randomValue <= epsilon)
                {
                    action = GetRandomValidAction();
                }
                else
                {

                    // Inicializar o valor máximo e a ação escolhida
                    foreach (KeyValuePair<int, decimal> entry in currentTable)
                    {
                        if (entry.Value > maxQ && validActions.Contains(entry.Key)) // Verificar se a ação é válida
                        {
                            maxQ = entry.Value;
                            action = entry.Key;
                        }
                    }
                }
                // Se não houver ação com valor Q definido, escolhe uma ação válida aleatoriamente
                if (action == -1)
                {
                    action = GetRandomValidAction();
                }

                return action;
            }
            else
            {

                foreach (int actionValid in validActions)
                    Console.WriteLine("actionValid:" + actionValid);

                Dictionary<int, decimal> currentTable = (currentPlayer == PlayerX) ? tableXmemory[state] : tableOmemory[state];
                int action = -1;
                decimal maxQ = decimal.MinValue;

                double doubleValue = random.NextDouble();
                decimal randomValue = (decimal)doubleValue;
                // Percorre a tabela de valores Q para encontrar a ação com o maior valor Q
                if (randomValue <= epsilon)
                {
                    action = GetRandomValidAction();
                }
                else
                {

                    foreach (KeyValuePair<int, decimal> entry in currentTable)
                    {
                        if (entry.Value > maxQ && validActions.Contains(entry.Key))
                        {
                            maxQ = entry.Value;
                            action = entry.Key;
                            Console.WriteLine(" maxQ: " + maxQ + " action: " + action);
                        }
                    }
                }
                // Se não houver ação com valor Q definido, escolhe uma ação válida aleatoriamente
                if (action == -1)
                {
                    action = GetRandomValidAction();
                }

                return action;
            }
        }

        // Obtém uma ação válida aleatória
        private int GetRandomValidAction()
        {
            List<int> validActions = GetValidActions();
            Random random = new Random();
            int index = random.Next(validActions.Count);
            return validActions[index];
        }

        // Obtém uma lista de ações válidas no tabuleiro atual
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

        // Aplica uma ação no tabuleiro
        private void ApplyAction(int action, int currentPlayer)
        {
            int row = (action - 1) / BoardSize;
            int col = (action - 1) % BoardSize;
            board[row, col] = currentPlayer;
        }
        
        private void ApplyActionSim(int action, int currentPlayer)
        {
            int row = (action - 1) / BoardSize;
            int col = (action - 1) % BoardSize;
            tempBoard[row, col] = currentPlayer;
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
                if(countCell == 5)
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
                if (countCell == 2)
                    return -3.0m;

                return -1.0m;
            }
            else if (IsDraw())
            {
                return 0m; // Empate
            }
            else
            {
                return 0m; // Jogo ainda não acabou
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

        // Obtém o maior valor Q para um determinado estado e jogador
        public decimal GetMaxQ(int state, int currentPlayer)
        {
            Dictionary<int, decimal> currentTable = (currentPlayer == PlayerX) ? tableXmemory[state] : tableOmemory[state];
            decimal maxQ = decimal.MinValue;
            
            foreach (decimal qValue in currentTable.Values)
            {
                if (qValue > maxQ)
                {
                    maxQ = qValue;
                }
            }

            return maxQ;
        }

        public decimal GetMaxQValidActions(int state, int currentPlayer)
        {
            Dictionary<int, decimal> currentTable = (currentPlayer == PlayerX) ? tableXmemory[state] : tableOmemory[state];
            decimal maxQ = decimal.MinValue;
            List<int> validActions = GetValidActions(); // Obtém as ações válidas
            foreach (int action in validActions)
            {
                if (currentTable.ContainsKey(action))
                {
                    decimal qValue = currentTable[action];
                    if (qValue > maxQ)
                    {
                        maxQ = qValue;
                    }
                }
            }

            return maxQ;
        }


        // Atualiza o valor Q para um estado e ação específicos
        public void UpdateQValue(int state, int action, decimal reward, decimal maxQ, int currentPlayer)
        {
            Dictionary<int, decimal> currentTable = (currentPlayer == PlayerX) ? tableX[state] : tableO[state];

            
            Dictionary<int, decimal> currentTableMemory = (currentPlayer == PlayerX) ? tableXmemory[state] : tableOmemory[state];

            decimal currentQ = currentTable[action];
            decimal updatedQ = currentQ + LearningRate * (reward + DiscountFactor * maxQ - currentQ);
            currentTable[action] = updatedQ;

            decimal currentQM = currentTableMemory[action];
            decimal updatedQM = currentQM + LearningRate * (reward + DiscountFactor * maxQ - currentQM);
            currentTableMemory[action] = updatedQM;
        }

        // Converte o estado do tabuleiro para um número
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
        
        public int GetGameStateAsNumberSim()
        {
            int gameState = 0;
            int factor = 1;

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    int cellValue = GetCellValue(tempBoard[i, j]);
                    gameState += cellValue * factor;
                    factor *= 3;
                }
            }

            return gameState;
        }

        // Obtém o valor de uma célula do tabuleiro
        private int GetCellValue(int cell)
        {
            if (cell == EmptyCell)
            {
                return 0; // Célula vazia
            }
            else if (cell == PlayerX)
            {
                return 1; // Jogador X
            }
            else if (cell == PlayerO)
            {
                return 2; // Jogador O
            }
            else
            {
                throw new Exception("Valor de célula inválido: " + cell);
            }
        }

        // Permite jogar contra um jogador humano
        public void PlayAgainstHuman()
        {
            int currentPlayer = PlayerX;
            decimal epsilon = 0.0m;

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

            int actionO = 0;
            int stateO = 0;

            int actionX = 0;
            int stateX = 0;

            List<int>validActionsMaxQ = new List<int>();

            while (!IsGameOver())
            {
                if (currentPlayer == PlayerX)
                {
                    if (contX > 0)
                        newStatesX[contX - 1] = GetGameStateAsNumber();

                    PrintBoard();

                    List<int> validActions = GetValidActions();
                    Console.WriteLine("Sua vez de jogar. Digite um número de 1 a 9 correspondente à posição desejada: ");
                    int position = int.Parse(Console.ReadLine());

                    if (!validActions.Contains(position))
                    {
                        Console.WriteLine("Posição inválida. Por favor, escolha outra posição.");
                        continue; // Volta para o início do loop e pede novamente uma posição válida
                    }

                    stateX = GetGameStateAsNumber();
                    actionX = position;

                    statesX[playsX] = stateX;
                    actionsX[playsX] = actionX;

                    ApplyAction(position, currentPlayer);

                    int newStateX = GetGameStateAsNumber();


                    rewardX = GetReward(currentPlayer);

                    rewardsX[playsX] = rewardX;

                    decimal maxQ = GetMaxQ(newStateX, currentPlayer);
                    UpdateQValue(stateX, actionX, rewardX, maxQ, currentPlayer);

                    playsX++;



                    contX++;

                    if (IsGameOver())
                    {
                        Console.WriteLine("IsGameOver");
                        decimal maxQO = GetMaxQ(stateX, currentPlayer * -1);
                        rewardO = GetReward(currentPlayer * -1);
                        UpdateQValue(stateO, actionO, rewardO, maxQO, currentPlayer * -1);
                    }
                }
                else
                {
                    if (contO > 0)
                        newStatesO[contO - 1] = GetGameStateAsNumber();

                    contO++;
                    stateO = GetGameStateAsNumber(); // Obtém o estado atual antes da jogada do agente
                    PrintQValuesForState(stateO, PlayerO);
                    List<int> validActions = GetValidActions(); // Obter as células disponíveis
                    List<int> possibleNewStates = new List<int>();



                    actionO = ChooseAction(stateO, currentPlayer, epsilon, false, validActions);

                    statesO[playsO] = stateO; // Armazena o estado atual
                    actionsO[playsO] = actionO; // Armazena a ação atual

                    PrintQValuesForState(stateO, PlayerO);
                    decimal maxQValidActions = GetMaxQValidActions(stateO, PlayerO);
                    // Mecanismo de punição de celulas anteriores 
                    
                    if (maxQValidActions < -0.8m)
                    {
                        decimal maxQValid = GetMaxQValidActions(statesO[contO - 1], PlayerO);
                        UpdateQValue(statesO[contO - 2], actionsO[contO - 2], -0.5m, maxQValid, PlayerO);
                    }
                    
                    int newStateO = GetGameStateAsNumber();
                    ApplyAction(actionO, currentPlayer);



                    validActions = GetValidActions();
                    tempBoard = board.Clone() as int[,];
                    foreach (int action in validActions)
                    {
                        board = tempBoard.Clone() as int[,];
                        ApplyAction(action, currentPlayer * -1);

                        // Obtém o número do estado após a simulação
                        int newStateSim = GetGameStateAsNumber();
                        possibleNewStates.Add(newStateSim);

                        decimal reward = GetReward(currentPlayer); // Obtém a recompensa para o jogador atual
                        decimal maxQSim = GetMaxQ(newStateSim, currentPlayer); // Obtém o valor máximo de Q para o novo estado
                        if (currentPlayer == PlayerO)
                        {
                            Console.WriteLine("GetReward(playsO): " + reward);
                        }
                        if (reward > 0 || reward < 0)
                            UpdateQValue(stateO, actionO, reward, maxQSim, currentPlayer);
                    }
                    board = tempBoard.Clone() as int[,];

                    rewardO = GetReward(currentPlayer);

                    rewardsO[playsO] = rewardO;

                    decimal maxQ = GetMaxQ(newStateO, currentPlayer);
                    if (rewardO > 0 || rewardO < 0)
                        UpdateQValue(stateO, actionO, rewardO, maxQ, currentPlayer);
                    Console.WriteLine("O computador escolheu a posição " + actionO);
                    playsO++;
                }

                currentPlayer *= -1;
            }

            int countCell = 1; // Inicializa a contagem como 0

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

            rewardO = GetReward(PlayerO);
            rewardX = GetReward(PlayerX);

            Console.WriteLine("GetReward(playsO): " + rewardO);
            for (int i = 0; i < contO; i++)
            {
                Console.WriteLine("O: ");
                Console.WriteLine("state: " + statesO[i] + " action: " + actionsO[i] + " newState: " + newStatesO[i]);
            }

            for (int i = 0; i < contX; i++)
            {
                Console.WriteLine("X: ");
                Console.WriteLine("state: " + statesX[i] + " action: " + actionsX[i] + " newState: " + newStatesX[i]);
            }
            /*
            if (rewardX > 0)
                for (int i = 0; i < contX; i++)
                {
                    Console.WriteLine("X: ");
                    decimal maxQ = GetMaxQ(newStatesX[i], PlayerX);
                    UpdateQValue(statesX[i], actionsX[i], rewardX, maxQ, PlayerX);
                }

            if (rewardO > 0)
                for (int i = 0; i < contO; i++)
                {
                    Console.WriteLine("O: ");
                    decimal maxQ = GetMaxQ(newStatesO[i], PlayerO);
                    UpdateQValue(statesO[i], actionsO[i], rewardO, maxQ, PlayerO);
                }

            
            */

            PrintBoard();

            if (IsWinner(PlayerX))
            {
                Console.WriteLine("Você venceu!");
            }
            else if (IsWinner(PlayerO))
            {
                Console.WriteLine("O computador venceu!");
            }
            else
            {
                Console.WriteLine("O jogo terminou em empate!");
            }
        }

        public void PrintQValuesForState(int state, int player)
        {
            if (player == PlayerX)
            {
                if (tableXmemory.ContainsKey(state))
                {
                    Console.WriteLine("Valores Q para o estado {0} (Jogador X):", state);
                    foreach (var action in tableXmemory[state])
                    {
                        Console.WriteLine("Ação: {0}, Valor Q: {1}", action.Key, action.Value);
                    }
                }
                else if (tableX.ContainsKey(state))
                {
                    Console.WriteLine("Valores Q para o estado {0} (Jogador X):", state);
                    foreach (var action in tableXmemory[state])
                    {
                        Console.WriteLine("Ação: {0}, Valor Q: {1}", action.Key, action.Value);
                    }
                }

                else
                {
                    Console.WriteLine("Não há valores Q para o estado {0}.", state);
                }
            }
            if (player == PlayerO)
            {
                if (tableOmemory.ContainsKey(state))
                {
                    Console.WriteLine("Valores Q para o estado {0} (Jogador O):", state);
                    foreach (var action in tableOmemory[state])
                    {
                        Console.WriteLine("Ação: {0}, Valor Q: {1}", action.Key, action.Value);
                    }
                }
                else if (tableX.ContainsKey(state))
                {
                    Console.WriteLine("Valores Q para o estado {0} (Jogador X):", state);
                    foreach (var action in tableXmemory[state])
                    {
                        Console.WriteLine("Ação: {0}, Valor Q: {1}", action.Key, action.Value);
                    }
                }

                else
                {
                    Console.WriteLine("Não há valores Q para o estado {0}.", state);
                }
            }
        }

        // Imprime o tabuleiro na tela
        private void PrintBoard()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    string cellValue = GetCellValueSymbol(board[i, j]);
                    Console.Write(cellValue + " ");
                }

                Console.WriteLine();
            }
        }

        private void PrintBoardSim()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    string cellValue = GetCellValueSymbol(tempBoard[i, j]);
                    Console.Write(cellValue + " ");
                }

                Console.WriteLine();
            }
        }

        // Obtém o símbolo correspondente a um valor de célula do tabuleiro
        private string GetCellValueSymbol(int cell)
        {
            if (cell == EmptyCell)
            {
                return "-";
            }
            else if (cell == PlayerX)
            {
                return "X";
            }
            else if (cell == PlayerO)
            {
                return "O";
            }
            else
            {
                throw new Exception("Valor de célula inválido: " + cell);
            }
        }

        // Imprime uma tabela de valores Q com um limite de estados
        private void PrintTable(Dictionary<int, Dictionary<int, decimal>> table, int limit)
        {
            int count = 0;
            foreach (var state in table)
            {
                if (count >= limit)
                    break;

                Console.WriteLine("Estado " + state.Key + ":");

                foreach (var action in state.Value)
                {
                    Console.WriteLine("   Ação " + action.Key + ": " + action.Value);
                }

                count++;
            }
        }

    }
}
