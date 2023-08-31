using System.Collections.Generic;
using System.Linq;
using KModkit;

public sealed class GameLogic
{
    public char[] Solution { get; private set; }
    public List<char> FirstStageResult { get; private set; }
    public List<char> SecondStageResult { get; private set; }
    
    private readonly KMBombInfo _bombInfo;
    private static readonly char[] Names = { 'P', 'B', 'G', 'D', 'E' };

    private bool _threeNamesBeforeBackup;
    
    public GameLogic(KMBombInfo bombInfo)
    {
        _bombInfo = bombInfo;
    }

    public void CalculateSolution()
    {
        Solution = Names.Shuffle().Take(3).ToArray();
    }

    public void FirstStage()
    {
        _threeNamesBeforeBackup = true;
        var result = new List<char>();
        // Perry
        var serialNumberDigits = _bombInfo.GetSerialNumberNumbers();
        var sum = serialNumberDigits.Sum();
        if (sum % 2 == 0 && result.Count != 3)
        {
            result.Add('P');
        }
        
        // Eddy
        var batteryCount = _bombInfo.GetBatteryCount();
        var batterySlotCount = _bombInfo.GetBatteryHolderCount();
        if (batteryCount >= 2 && batterySlotCount >= 2 && result.Count != 3)
        {
            result.Add('E');
        }

        // Barry
        var duplicatePorts = _bombInfo.CountDuplicatePorts();
        if (duplicatePorts != 0 && result.Count != 3)
        {
            result.Add('B');
        }
        
        // Darrel
        if (_bombInfo.IsPortPresent("RJ45") && result.Count != 3)
        {
            result.Add('D');
        }
        
        // Gary
        if (_bombInfo.GetOnIndicators().Count() >= _bombInfo.GetOffIndicators().Count() && result.Count != 3)
        {
            result.Add('G');
        }

        if (result.Count < 3)
        {
            _threeNamesBeforeBackup = false;
        }

        var backupNames = new[] { 'D', 'G', 'E' };

        FirstStageResult = result.Concat(backupNames).Distinct().Take(3).ToList();
    }

    public void SecondStage()
    {
        var result = new List<char>();
        for(var i = 0; i < 3; i++)
        {
            switch (FirstStageResult[i])
            {
                case 'P':
                    result.Add(SecondStageTable(i, new [] { 'G', 'E', 'B', 'D' }));
                    break;
                
                case 'B':
                    result.Add(SecondStageTable(i, new [] { 'D', 'P', 'G', 'E' }));
                    break;
                
                case 'G':
                    result.Add(SecondStageTable(i, new [] { 'E', 'B', 'D', 'P' }));
                    break;
                
                case 'D':
                    result.Add(SecondStageTable(i, new [] { 'P', 'G', 'E', 'B' }));
                    break;
                
                case 'E':
                    result.Add(SecondStageTable(i, new [] { 'B', 'D', 'P', 'G' }));
                    break;
            }
        }
        
        SecondStageResult = result.Concat(new[] {'P', 'B', 'G', 'D', 'E'}).Distinct().Take(3).ToList();
    }

    private char SecondStageTable(int i, IList<char> table)
    {
        switch (i)
        {
            case 0:
                return table[0];
            case 2:
                return table[1];
            default:
                return _threeNamesBeforeBackup ? table[2] : table[3];
        }
    }
}