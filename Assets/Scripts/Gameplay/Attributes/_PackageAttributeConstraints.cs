// FUCK clean code we going booga mode now!!
using System;
using System.Collections.Generic;

public static class PackageAttributeConstraints
{
    // weight
    public static readonly int[] WeightLimits = new[] { 1, 20 };
    public const double WeightDeviationBaseChance = 0.25;
    public const int WeightMaxDeviationAmount = 5;

    // shipper
    public static readonly string[] ValidShippers = new[] { "Amazon", "WeBay", "Fexed" };
    public static readonly string[] InvalidShippers = new[] { "AmazonFake", "WeBayFake" };
    public const double ShipperForgedBaseChance = 0.2;

    // shipTo and from
    // (thanks gpt for the addresses)
    public static readonly int[] AddressLineOneNumLimits = new[] { 1, 999 };
    public static readonly string[] AddressLineOneMid = new[]
    {
    "North", "South", "East", "West", "Old", "New", "Upper", "Lower",
    "Lake", "River", "Hill", "Valley", "Oak", "Pine", "Maple", "Cedar",
    "Elm", "Birch", "Willow", "Aspen", "Spruce", "Cherry", "Walnut", "Hickory",
    "Sycamore", "Redwood", "Sequoia", "Juniper", "Ash", "Beech", "Poplar", "Fir",
    "Garden", "Park", "Plaza", "Heights", "Terrace", "Village", "Center", "Square",
    "Sunset", "Sunrise", "Moonlight", "Starlight", "Spring", "Summer", "Autumn", "Winter",
    "Green", "Blue", "Golden", "Silver", "Crystal", "Shadow", "Bright", "Dark",
    "Stone", "Rock", "Pebble", "Boulder", "Fox", "Wolf", "Bear", "Deer",
    "Eagle", "Hawk", "Raven", "Owl", "Lion", "Tiger", "Panther", "Cougar",
    "Horse", "Mustang", "Colt", "Pony", "Brook", "Creek", "Stream", "Run",
    "Meadow", "Field", "Pasture", "Prairie", "Canyon", "Ridge", "Cliff", "Bluff",
    "Vista", "View", "Lookout", "Point", "Harbor", "Bay", "Cove", "Shore",
    "Island", "Harborview", "Seaside", "Ocean" };
    public static readonly string[] AddressLineOneEnd = new[] {"St", "Rd", "Ave", "Blvd", "Ln", "Dr", "Ct", "Pl", "Cir", "Way" };
    public static readonly string[] Regions = new[] { "West Market", "East Industrial", "North Settlement", "South Quarry", "Old Port City" };
    public static readonly string[] BannedRegionOnSecondDay = new[] { "Old Port City" };
    public static readonly string[] BannedRegionsOnThirdDay = new[] { "Old Port City"};
    public const double InvalidZIPCodeBaseChance = 0.1; // first number of zipcode must align with the index of the region, other 4 are irrelevant

    // date
    public const int DayOneDate = 4897;
    public const int BannedDatesAfterDayThree = 4898;
    public const int DateDeviationFromCurrent = 5;

    // remarks
    public static readonly string[] UselessRemarks = new[] { 
        "We're watching you.",
        "Please make sure this reaches my grandson.",
        "What?",
        "Leave it on the doorstep, please.",
        "My neighbor stinks.",
        "I'm hungryyyy",
        "Please insert $5 into this package through the slit.",
        "Forgive me...",
        "If lost, call XXX-XXX-XXXX",
        "I love you, Maria",
        "Handle with care!!!!!!",
        "Ok",
        "N/A",
        "Yeah",
        "Thank you for the business.",
        "NO REFUNDS",
        "FRAGILE",
        "What do i put here?",
        "Leave on doorstep",
        "Leave on chair",
        "leave on chair",
        "leave on doormat",
        "Leave on doormat",
        "Leave on chair. The brown one",
        "Have a good day!",
        "Thank you: )",
        "thank you",
        "paypal.me/needmoneyplsgive",
        "Deliver at the back door",
        "Please respond to my messages, son.",
        "I have 3 months to live",
        ": p",
        ": o",
        ": (",
        ": )",
    };
    public static readonly string[] ThreateningRemarks = new[] { 
        "Death to the regime!!!", 
        "Fuck President Osmar!!",
        "Open this package, bitch. See what happens.",
        "If you break my shit im going to kill you",
        "Viva la revolution!",
        "Recruiting for the Liberation Front -- contact XXX-XXX-XXXX",
        "I can't wait to see that rat Osmar's head exploding on live television. Kino kino",
        "JOIN THE LIBERATION FRONT",
        "Looking forward to pissing on your grave in a week",
    };
    public const double ThreateningRemarkBaseChance = 0.08;

    public static readonly string[] amazonGood = new[] { "13579", "97531", "113355", "579", "7331", "911", "157", "33333", "975", "7777" };
    public static readonly string[] amazonBad = new[] { "24680", "135792", "8080", "113354", "429", "10203", "6017", "2201", "8642", "1112" };
    public static readonly string[] weBayGood = new[] { "A1B2C3D4E5F6", "ABCDEF123456", "1A2B3C4D5E6F", "W3E4B5A6Y7Z8", "Z9X8C7V6B5N4", "M1N2B3V4C5X6", "R3T2Y5U6I7O8", "A9S8D7F6G5H4", "Q1W2E3R4T5Y6", "123ABC456DEF" };
    public static readonly string[] weBayBad = new[] { "ABC123", "123456789012", "AABBCCDDEEFF", "1A2B3C4D5E", "ABC123456", "A1B2C3D4E5F", "1A2B3C4D5E6G7", "A1B2C3D4E5F6G", "A1B2C3D4E5F5G6", "12345ABCDE" };
    public static readonly string[] fexedGood = new[] { "1234", "A5674", "FEDEX4", "9876543214", "ZXCVMNB4", "IDNUM4", "SHIPIT4", "A1B2C3D4", "000000004", "TRK9999999994" };
    public static readonly string[] fexedBad = new[] { "1235", "A5670", "FEDEX3", "9876543219", "ZXCVMNB7", "IDNUM2", "SHIPIT0", "A1B2C3D9", "000000001", "TRK9999999997" };
}