using Assets.Scripts.Gameplay.Attributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using PAC = PackageAttributeConstraints;
using Assets.Scripts.Util;
using Assets.Scripts.Events;
using DG.Tweening;
using System.Threading.Tasks;
/// <summary>
/// Handles the spawning of packages and the general game flow.
/// </summary>
public class LevelManager : MonoBehaviour
{
    private int _packagesLeft;
    public Queue<Package> packages;
    private List<IDisposable> _connections;
    private Package _activePackage;
    private GameObject _activePackageModel;
    private bool _running;
    private InputSystem_Actions _actions;
    [SerializeField] private GameObject _c4Model;

    private int _currentPage, _availablePages;

    public int violations;
    public int maxViolations;

    void Start()
    {
        GameManager.Inst.CurrentDay = Days.DayThree;
        if (GameManager.Inst == null)
        {
            Utils.Talk(new("(SYSTEM) GameManager not initialized. Run from the bootstrapper idiot", Color: ChatColors.Angry));
            StartDay(Days.DayOne);
        }
        else
            StartDay(GameManager.Inst.CurrentDay);
    }

    private void StartDay(Days day)
    {
        Utils.Fade(new(Color.black, 1f, FadeDirection.Out));

        var pg = PackageGenerator.Inst;
        packages = new Queue<Package>();
        violations = 0;
        _packagesLeft = -2;
        _running = true;
        _availablePages = 0;
        _currentPage = 0;
        switch (day)
        {
            case Days.DayOne:
                maxViolations = 2;
                _availablePages = 2;
                //so we dont go to gameend during the dialogue sequence
                
                Utils.Talk(new("*yawn* What up rookie."));
                Utils.Defer(3, () => {
                    Utils.Talk(new("Welcome to your new job! Take a look around. <i>(MOUSE to look. A/D to turn.)</i>", 3));
                });
                Utils.Defer(6, () => {
                    Utils.TalkDeferred(3, new("Your responsibility is to examine these packages, tossing out sketchy ones."));
                    Utils.TalkDeferred(8, new("You have a scale and a manual to get started."));
                    Utils.TalkDeferred(12, new("<i>F to pull up the manual.</i>"));
                    packages.Enqueue(
                       new Package(true,
                       pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day),
                       pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateGoodShipper(), pg.GenerateGoodID(),
                       OnSpawnedCallback: (obj) =>
                       {
                           Utils.Talk(new("Oh, and here's the first package."));
                           Utils.TalkDeferred(18, new("Take your time before you make your decision"));
                           Utils.TalkDeferred(21, new("You DON'T want to make the wrong one", Color: ChatColors.Angry));
                           Utils.TalkDeferred(24, new("btw use your mouse to examine"));
                           Utils.Defer(24, () =>  GameManager.Inst.SetFlag(StoryFlags.EnablePickup) );
                           Utils.TalkDeferred(30, new("Approve by placing the package on the next conveyer otherwise, deny with E"));
                           //Utils.Talk(new("test, making sure OnSpawnedCallback works"));
                           //this puts the package ontop of the conveyor window lol
                           obj.transform.position += new Vector3(0, 0, 0);
                       },
                       OnProcessedCallback: (obj, accepted) => {
                           Utils.Talk(accepted
                               ? new("Good work. You're doing better than the last guy already.")
                               : new("...yikes... who the hell hired you?", Color: ChatColors.Angry)
                               );
                           if (!accepted) GameManager.Inst.SetFlag(StoryFlags.FailedFirstPackage);
                       }));
                    packages.Enqueue(new Package(false,
                        pg.GenerateBadWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day),
                        pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(),  pg.GenerateGoodShipper(), pg.GenerateGoodID(),
                        OnSpawnedCallback: (obj) =>
                        {
                            if (GameManager.Inst.GetFlag(StoryFlags.FailedFirstPackage))
                            {
                                Utils.TalkDeferred(3, new("...Try not to mess up this time.", Color: ChatColors.Disappointed));
                            }
                        },
                       OnProcessedCallback: (obj, accepted) => {
                           Utils.Talk(!accepted
                               ? new("Nice job using your scale.")
                               : new("That was a freebie, man. Check the scale next time.", Color: ChatColors.Disappointed)
                               );
                           if (!accepted) GameManager.Inst.SetFlag(StoryFlags.FailedFirstPackage);
                       }));
                    Debug.Log(packages.Peek().WeightPair);
                    packages.Enqueue(new Package(false, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(),  pg.GenerateBadShipper(), pg.GenerateGoodID(),
                        OnSpawnedCallback: (obj) =>
                        {
                            Utils.TalkDeferred(3, new("Listen pal,"));
                            Utils.TalkDeferred(5, new("just stay focused and you'll do fine."));
                            Utils.TalkDeferred(8, new("I'll check in with ya tomorrow. *click*"));

                        }));
                    packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(),  pg.GenerateGoodShipper(), pg.GenerateGoodID()));
                    packages.Enqueue(new Package(false, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateBadRemark(), pg.GenerateGoodShipper(), pg.GenerateGoodID()));
                    packages.Enqueue(new Package(false, pg.GenerateGoodWeightPair(), pg.GenerateBadZipAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateGoodShipper(), pg.GenerateGoodID(),
                        OnProcessedCallback: (obj, accepted) =>
                        {
                            Utils.Talk(!accepted
                                ? new("Nice catch")
                                : new("Be sure to check that sender and ", Color: ChatColors.Disappointed)
                                );
                            if (!accepted) GameManager.Inst.SetFlag(StoryFlags.FailedFirstPackage);
                        }));
                    _packagesLeft = packages.Count;
                });
                break;
            case Days.DayTwo:
                //for testing
                GameManager.Inst.SetFlag(StoryFlags.EnablePickup);
                maxViolations = 1;
                _availablePages = 3;
                Utils.Talk(new("...You seen the news today?"));
                Utils.Defer(3, () =>
                {
                    Utils.TalkDeferred(4, new("A delivery driver's van got blown up"));
                    Utils.TalkDeferred(8, new("SOMEONE made a mistake and let an explosive get through. ", Color: ChatColors.Angry));
                    Utils.TalkDeferred(11, new("Does that someone sound familiar to you?"));
                    Utils.TalkDeferred(14, new("*sigh* Whatever. Don't let it happen twice, cause every time it happens,"));
                    Utils.TalkDeferred(23, new("we get more paperwork to deal with."));
                    Utils.TalkDeferred(27, new("See that manual? There's a second page on it now. Thanks to you."));
                    Utils.TalkDeferred(31, new("So make sure you're checking those too."));
                    packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateGoodShipper(), pg.GenerateGoodID()));
                    packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateGoodShipper(), pg.GenerateGoodID()));
                    packages.Enqueue(new Package(false, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateGoodShipper(), pg.GenerateBadID()));
                    packages.Enqueue(new Package(false, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateBadRemark(), pg.GenerateGoodShipper(), pg.GenerateGoodID()));
                    packages.Enqueue(new Package(false, pg.GenerateGoodWeightPair(), pg.GenerateBadZipAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateGoodShipper(), pg.GenerateGoodID()));
                    packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateGoodShipper(), pg.GenerateGoodID()));
                    packages.Enqueue(new Package(false, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateBadRegionAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateGoodShipper(), pg.GenerateGoodID()));
                    _packagesLeft = packages.Count;
                });
                
                break;
            case Days.DayThree:
                GameManager.Inst.SetFlag(StoryFlags.EnablePickup);
                maxViolations = 1;
                _availablePages = 4;

                Utils.Talk(new("<i>Bomberman!</i> How's it going?"));
                Utils.TalkDeferred(4, new("..Just joking. God. Whatever"));
                Utils.TalkDeferred(8, new("Anyways, I'm calling in sick today. And probably till this whole bomb thing"));
                Utils.TalkDeferred(12, new("boils over. I'm seeing things, man.. I swear there's someone looking at me,"));
                Utils.TalkDeferred(15, new("right through the window."));
                Utils.TalkDeferred(19, new("So, you're on your own! Don't forget to read the third page of the manual."));

                packages.Enqueue(new Package(
                    false, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), "<b>Pick up the phone. It is in your best interest to so do.</b>", pg.GenerateGoodShipper(), pg.GenerateGoodID(),
                    OnTelephonePickedUpCallback: (obj, first) =>
                    {
                        if (!first) return;
                        GameManager.Inst.SetFlag(StoryFlags.AcknowledgedBombPackage);
                        Utils.Talk(new("Inspector... inspector. I've been giving you a <i>rough</i> time recently, huh?", Color: ChatColors.Angry));
                        Utils.TalkDeferred(4, new("But worry not. You're in for a surprise today; make sure you take it well.", Color: ChatColors.Angry));
                        Utils.TalkDeferred(7, new("No hard feelings", Color: ChatColors.Angry));
                    },
                    OnProcessedCallback: (obj, accepted) =>
                    {
                        if (!GameManager.Inst.GetFlag(StoryFlags.AcknowledgedBombPackage))
                        {
                            GameManager.Inst.SetFlag(StoryFlags.AcknowledgedBombPackage);
                            Utils.Talk(new("*loudspeaker buzz*", Color: ChatColors.Angry));
                            Utils.TalkDeferred(3, new("Inspector... inspector. I've been giving you a <i>rough</i> time recently, huh?", Color: ChatColors.Angry));
                            Utils.TalkDeferred(7, new("But worry not. You're in for a surprise today; make sure you take it well.", Color: ChatColors.Angry));
                            Utils.TalkDeferred(11, new("No hard feelings", Color: ChatColors.Angry));
                        }
                    }));
                packages.Enqueue(new Package(false, pg.GenerateBadWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateGoodShipper(), pg.GenerateGoodID()));
                packages.Enqueue(new Package(true, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateGoodAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateGoodShipper(), pg.GenerateGoodID()));
                packages.Enqueue(new Package(false, pg.GenerateGoodWeightPair(), pg.GenerateGoodAddress(day), pg.GenerateBadRegionAddress(day), pg.GetCurrentDate(day).ToString(), pg.GenerateGoodRemark(), pg.GenerateGoodShipper(), pg.GenerateGoodID(), 
                    OnPickedUpCallback: (obj, first) => {
                        var info = obj.GetComponent<PackageInfo>();
                        info.Processed = true;

                        obj.SetActive(false);
                        _c4Model.SetActive(true);
                        if (!first) return;
                        Utils.Talk(new("*ring ring*", Color: ChatColors.Feds));
                        Utils.TalkDeferred(3, new("This is Agent John from the FBI speaking.", Color: ChatColors.Feds));
                        Utils.TalkDeferred(6, new("I see you have a problem on your hands.", Color: ChatColors.Feds));
                        Utils.TalkDeferred(9, new("Fear not, there's an old manual we have lying around", Color: ChatColors.Feds));
                        Utils.TalkDeferred(12, new("that may be helpful for this situation.", Color: ChatColors.Feds));
                        Utils.TalkDeferred(15, new("Problem is, I don't know which manual it is.", Color: ChatColors.Feds));
                        Utils.TalkDeferred(18, new("But whatever you do.. DO NOT throw it away or send it through the conveyor.", Color: ChatColors.Feds));
                        Utils.TalkDeferred(21, new("The machinery here could easily pay for your bloodline five times over.", Color: ChatColors.Feds));
                        Utils.TalkDeferred(24, new("*kachunk* Anyways, I've faxed you the defusal guide. Seems like", Color: ChatColors.Feds));
                        Utils.TalkDeferred(27, new("you'll need to cut a wire -- just, I don't know which wire that'd be.", Color: ChatColors.Feds));
                        Utils.TalkDeferred(30, new("Either you figure it out", Color: ChatColors.Feds));
                        Utils.Defer(30, () => _availablePages = 5);
                        Utils.TalkDeferred(33, new("or you won't need to worry about figuring it out anymore.", Color: ChatColors.Feds));
                        Utils.TalkDeferred(36, new("By the way, there's like a minute left till that detonates. Have fun", Color: ChatColors.Feds));

                        Utils.Defer(60 + 36, () => EndDay(Days.BombExploded));

                        EventBus.Publish<OpenC4Event>(new(
                            Success: () =>
                            {
                                _c4Model.SetActive(false);
                                Utils.Talk(new("*beeeep*"));
                                Utils.Defer(2,() => EndDay(Days.DayThree));
                            },
                            Fail: () =>
                            {
                                EndDay(Days.BombExploded);
                            }));
                            
                    }));

                _packagesLeft = packages.Count;
                break;
            case Days.GameEnd:
                Utils.Fade(new(Color.black, 1f));
                Utils.Talk(new("This was our first project from our first semester learning Unity.", 999));
                Utils.Talk(new("We hope you had fun playing our shitty Papers Please clone.", 999));
                Utils.TalkDeferred(3, new("<b>END</b>", 999));
                Utils.TalkDeferred(3, new("<b>Thanks for playing</b>", 999));
                Utils.TalkDeferred(3, new("Credits: Juan, Austin, Justin", 999));

                break;
            default:
                Debug.LogError(day.ToString());
                break;
        }

    }

    private void Awake()
    {
        _actions = new();
    }

    private void OnEnable()
    {
        _connections = new()
        {
            EventBus.Subscribe<DecisionMadeEvent>(OnDecisionMade),
            EventBus.Subscribe<PackagePickedUpEvent>(OnPackagePickedUp),
            EventBus.Subscribe<TelephonePickupEvent>(OnTelephonePickup),
        };
        _actions.Player.Enable();
        _actions.Player.Manual.started += OnMaunalFlip;

    }

    private void OnDisable()
    {
        _connections.ForEach(x => x.Dispose());
        _connections.Clear();
        _actions.Player.Disable();
        _actions.Player.Manual.started -= OnMaunalFlip;
    }

    private void OnMaunalFlip(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _currentPage = (_currentPage + 1) % _availablePages;
        EventBus.Publish<ManualFlippedEvent>(new(_currentPage));
    }

    private void OnPackagePickedUp(PackagePickedUpEvent e)
    {
        if (_activePackage == null || _activePackageModel == null)
            return;
        _activePackage.OnPickedUpCallback?.Invoke(_activePackageModel, !_activePackage.PickedUp);
        _activePackage = _activePackage with { PickedUp = true }; 
    }

    private void OnTelephonePickup(TelephonePickupEvent e)
    {
        if (_activePackage == null || _activePackageModel == null)
            return;
        Utils.Talk(new("*ring ring*"));
        _activePackage.OnTelephonePickedUpCallback?.Invoke(_activePackageModel, !_activePackage.NumberCalled);
        _activePackage = _activePackage with { NumberCalled = true };
    }

    // Update is called once per frame
    void Update()
    {
        CheckPackage();
    }

    public int GetRemainingPackages() => _packagesLeft;

    private void OnDecisionMade(DecisionMadeEvent msg)
    {
        if (_activePackage == null || _activePackageModel == null)
        {
            //Utils.Talk(new("how the hell did you call this? there is no package to make a decision on", Color: ChatColors.Angry));
            return;
        }

        if (!_activePackageModel.TryGetComponent<PackageInfo>(out var info))
            Debug.LogError("no package info on the active package model");

        Utils.Talk(new("<i>(processing)</i>", 0));
        info.Processed = true;

        if (msg.Accepted != _activePackage.Valid)
        {
            violations++;
            EventBus.Publish<IncorrectChoiceEvent>(new(violations));
            if (violations > maxViolations)
            {
                EndDay(Days.FiredForSuckingAtJob);
            }
        }
        else
        {
            EventBus.Publish<CorrectChoiceEvent>(new());
        }

        if (_activePackage.OnProcessedCallback is not null)
            _activePackage.OnProcessedCallback(_activePackageModel, msg.Accepted);

        Destroy(_activePackageModel, 1);
        _activePackage = null;
        _activePackageModel = null;
    }

    private void CheckPackage()
    {
        if (_packagesLeft == -1)
        {
            EndDay();
        } else if(_packagesLeft == -2)
        {
            //do nothin. this is prior to game start
        } else if (_packagesLeft >= 0 && CountObjectsWithTag("Package") == 0)
        {
            if (_packagesLeft != 0)
            {
                _activePackage = packages.Dequeue();
                _activePackageModel = PackageGenerator.Inst.SpawnPackage(_activePackage);
                if (_activePackage.OnSpawnedCallback is not null)
                    _activePackage.OnSpawnedCallback(_activePackageModel);
            }
            _packagesLeft--;
        }

    }
    public int CountObjectsWithTag(string tag)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        return taggedObjects.Length;
    }

    public void EndDay(Days? day = null)
    {
        if (!_running) return;
        _running = false;
        day ??= GameManager.Inst.CurrentDay;

        switch (day)
        {
            case Days.DayOne:
                Utils.Fade(new(Color.black, 2));
                Utils.Talk(new("(You finish your first day without incident.)"));
                Utils.TalkDeferred(3, new("(You arrive home, eat a microwaved dinner, and lay on the bed for a nap.)"));
                Utils.TalkDeferred(6, new("(Maybe.. scanning packages all day isn't so bad.)"));

                //BOOM happens here
                EventBus.Publish<PlayExplosionEvent>(new());

                Utils.TalkDeferred(10, new("(Screams can be heard from the adjacent house to you. You rush out to investigate.)"));
                Utils.TalkDeferred(13, new("(Was it a gas leak? A firecracker? No... it doesn't appear to be either.)"));
                Utils.TalkDeferred(16, new("(Looking out on the street, your eyes are drawn to a truck. A delivery truck.)"));
                Utils.TalkDeferred(19, new("(...and the gored remains of the mailman)"));
                Utils.TalkDeferred(21, new("(...who only seconds ago had been routinely delivering a package to your neighbors.)"));
                Utils.TalkDeferred(24, new("(You may not be the brightest, but even a dim bulb can put together two and two.)", 4));
                Utils.Defer(30,() => GameManager.Inst.LoadLevel(Days.DayTwo));
                break;
            case Days.DayTwo:
                Utils.Fade(new(Color.black, 2));
                Utils.Talk(new("(Day two goes by. Work is getting... stressful.)"));
                Utils.TalkDeferred(3, new("(Rumors of the bomber are the talk of the town.)"));
                Utils.TalkDeferred(6, new("(Local news outlets are rushing to grasp at any straw they can find.)"));
                Utils.TalkDeferred(9, new("(On the television, a reporter is speaking to the camera, speculating on the perpetrator of last night's attack.)"));
                Utils.TalkDeferred(9, new("(Including you, the new inspector who conveniently let a bomb through on day one.)"));
                Utils.TalkDeferred(12, new("(Tired of the slander, you reach for the remote to turn off the television.)"));
                // BOOM happens here.
                Utils.Defer(13, () => EventBus.Publish<PlayExplosionEvent>(new()));
                Utils.TalkDeferred(14, new("..."));
                Utils.TalkDeferred(15, new("(The camera was shattered by the explosion, sparing hundreds of viewers the sight of a gruesome scene.)"));
                Utils.TalkDeferred(18, new("(It's easier the second time.)"));
                // BOOM happens here again.
                Utils.Defer(20, () => EventBus.Publish<PlayExplosionEvent>(new()));

                Utils.TalkDeferred(21, new("(...and the third.)"));
                // BOOM happens here again.
                Utils.Defer(23, () => EventBus.Publish<PlayExplosionEvent>(new()));

                Utils.TalkDeferred(24, new("(Tomorrow's gonna be a long day.)", 4));
                Utils.Defer(30, () => GameManager.Inst.LoadLevel(Days.DayThree));
                break;
            case Days.DayThree:
                Utils.Fade(new(Color.black, 3));
                Utils.Talk(new("(You successfully defuse the bomb, despite shoddy instructions and pressure under time.)"));
                Utils.TalkDeferred(3, new("(Police swarm the office, collecting the defused bomb for further examination.)"));
                Utils.TalkDeferred(6, new("(The weekend could not arrive any later.)"));
                Utils.TalkDeferred(9, new("You can finally put off the thought of the bomber, at least till Monday.)"));
                Utils.TalkDeferred(12, new("(As you sit down on your sofa, your phone rings, buried under a pillow.)"));
                Utils.TalkDeferred(15, new("(Putting the phone up to your ear, you answer - wait a second...)"));
                Utils.TalkDeferred(18, new("(This isn't your phone. And it's heavier than it should be.)"));
                Utils.TalkDeferred(21, new("(Far, far heavier.)"));
                Utils.TalkDeferred(24, new("(A simple, pre-recorded message comes from the speaker.)"));
                Utils.TalkDeferred(27, new("('Are you done playing hero yet?'", 4));
                Utils.Defer(29, () => EventBus.Publish<PlayExplosionEvent>(new()));
                Utils.Defer(33, () => GameManager.Inst.LoadLevel(Days.GameEnd));
                break;
            case Days.FiredForSuckingAtJob:
                DOTween.KillAll();
                Utils.Fade(new(Color.black, 2));
                Utils.Talk(new("*buzz* Inspector. Management is quite concerned with your performance.", 9, Color: ChatColors.Angry));
                Utils.TalkDeferred(3, new("As you should know, the Postal Service does not take well to inefficiencies.", 6, Color: ChatColors.Angry));
                Utils.TalkDeferred(6, new("...Turn in your ID at the office immediately.", 3, Color: ChatColors.Angry));
                Utils.TalkDeferred(9, new("(Do better next time.)", Color: ChatColors.Angry));
                Utils.Defer(14,() => GameManager.Inst.LoadLevel());
                break;
            case Days.BombExploded:
                EventBus.Publish<PlayExplosionEvent>(new());
                Utils.Fade(new(Color.black, 0.01f));
                Utils.Talk(new("Well shit.", 3, Color: ChatColors.Disappointed));
                Utils.Defer(5, () => GameManager.Inst.LoadLevel());
                break;
            default:
                Debug.LogError(day.ToString());
                GameManager.Inst.LoadLevel(Days.DayOne);
                break;
        }
    }
}
