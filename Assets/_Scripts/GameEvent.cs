public abstract class GameEvent { }

public class CurrencyChangeGameEvent : GameEvent
{
    public int amount; 
    public CurrencyType currencyType;

    public CurrencyChangeGameEvent(int amount, CurrencyType currencyType)
    {
        this.amount = amount;
        this.currencyType = currencyType;
    }
}

public class NotEnoughCurrencyGameEvent : GameEvent
{
    public int amount; 
    public CurrencyType currencyType;

    public NotEnoughCurrencyGameEvent(int amount, CurrencyType currencyType)
    {
        this.amount = amount;
        this.currencyType = currencyType;
    }
}

public class EnoughCurrencyGameEvent : GameEvent
{
        
}

public class XPAddedGameEvent : GameEvent
{
    public int amount; 

    public XPAddedGameEvent(int amount)
    {
        this.amount = amount;
    }
}

public class LevelChangedGameEvent : GameEvent
{
    public int newLvl;

    public LevelChangedGameEvent(int currLvl)
    {
        newLvl = currLvl;
    }
}
public class BuildingGameEvent : GameEvent
{
    public string BuildingName;

    public BuildingGameEvent(string name)
    {
        BuildingName = name;
    }
}
public class PlantingGameEvent : GameEvent
{
    public string PlantingName;

    public PlantingGameEvent(string name)
    {
        PlantingName = name;
    }
}
public class HarvestingGameEvent : GameEvent
{
    public string HarvestingName;

    public HarvestingGameEvent(string name)
    {
        HarvestingName = name;
    }
}
public class ProductionGameEvent : GameEvent
{
    public string ProductionName;

    public ProductionGameEvent(string name)
    {
        ProductionName = name;
    }
}
public class BreedingGameEvent : GameEvent
{
    public string BreedingName;
    public BreedingGameEvent(string name)
    {
        BreedingName = name;
    }
}
public class HatchingGameEvent : GameEvent
{
    public string HatchingName;
    public HatchingGameEvent(string name)
    {
        HatchingName = name;
    }
}