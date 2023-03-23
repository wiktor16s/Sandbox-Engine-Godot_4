namespace SandboxEngine;

public enum EMaterial : uint
{
    SAND = EColors.YELLOW,
    STONE = EColors.WHITE,
    NONE = EColors.BLACK,
    WATER = EColors.BLUE,
    UNKNOWN = EColors.RED
}

public enum EColors : uint
{
    YELLOW = 4294902015,
    WHITE = 4294967295,
    BLACK = 255,
    BLUE = 65535,
    RED = 4278190335
}