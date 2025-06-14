namespace RockPaperScissorsLizardSpockGame.Application.Helpers;

// This class contais random fun fact generator as I thought it might be intesting add-on to the game for someone not familiar with RPSLS Game.
public static class PlayGameResponseHelpers
{
    public static string[] FunFacts = new[]
    {
        "This game was invented by Sam Kass and Karen Bryla.",
        "Made famous by The Bin Bang Theory TV show.",
        "Every move defeats exactly two others. Balanced gameplay!",
        "There are 10 matchups in RPSLS, versus 3 in RPS.",
        "Spock is the only move named after a person!",
        "Spock vaporizes Rock!",
        "Lizard poisons Spock... why? Nobody knows.",
        "Scissors decapitates Lizard!",
        "Paper disproves Spock!",
        "Rock crushes Scissors!",
        "Rock Paper Scissors originated in ancient China, around 206 BCE - 220 CE, during the Han Dynasty, where it was known as Shoushilling",
        "It spread to Japan where it became known as Jan-Ken - still the dominant version in modern Japan.",
        "Jan-Ken - Japan's version of RPS uses same gestures, but with a different set of meanings and rules.",
        "The logic of RPSLS is non-transitive - meaning there is not single best move; every choice wins and looses equally.",
        "Several AI research projects have used RPS and RPSLS to study human pattern prediction and bluffing behaviour.",
        "RPSLS expands the original 3 gestures to 5, reducing tie probability from 33.3% to 20%.",
        "RPSLS was invented in the early 2000s to fix the tie problem in standard RPS.",
        "Psychologist have observed that people rarely chose moves randomly - they form predictable patterns, e.g after losing with rock, they switch to paper.",
        "Competitive RPS tournaments include mind-games and bluffing tactics, similar to poker - even in RPSLS variants."
    };
    public static string GetRandomFunFact() => FunFacts[new Random().Next(FunFacts.Length)];
}