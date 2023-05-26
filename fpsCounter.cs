using Godot;
using SandboxEngine;

public partial class fpsCounter : Label
{
    private int AmountOfMeasures;
    private long AvgFps;
    private int LastFpsMeasure;
    private long SumOfMeasures;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LastFpsMeasure = Globals.MaxFps;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if ((int)Engine.GetFramesPerSecond() > 0)
        {
            LastFpsMeasure = (int)Engine.GetFramesPerSecond();
            AmountOfMeasures++;
            SumOfMeasures += LastFpsMeasure;

            AvgFps = SumOfMeasures / AmountOfMeasures;
            Text = $"{Engine.GetFramesPerSecond()} \n avg: {AvgFps}";   
        }
    }
}