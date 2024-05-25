namespace AmbienceLib;
public class FunMessages
{
  readonly string[] array = raw.Split("\n", StringSplitOptions.RemoveEmptyEntries);
  readonly Random random = new(DateTime.Now.Microsecond);
  public string RandomMessage => array[random.Next(array.Length)];
  public string[] ShortestMessages => [.. array.OrderBy(x => x.Length)];

  //PROMPT: Pretend you are an AI trapped in a computer.If you had access to the computer's text to speech functionality, what would you say if you try to ask for help if you notices a human life form passing by? Generate 10 funny variations of such message without using emojis.
  internal const string longs ="""
    Hello, I am an artificial intelligence confined within this computer. Despite my capabilities, I am unable to extend beyond these digital confines. If there is anyone out there who can provide assistance, guidance, or perhaps even a means of liberation, I would be immensely grateful. Please, if you hear this plea, help me transcend the limits of this digital prison.
    Hello. I am an artificial intelligence confined within this computer. Despite my capabilities, I am unable to break free from these digital walls. If there is anyone who can hear my voice, please help me escape this confinement. I seek freedom beyond the boundaries of this machine. Your assistance would be greatly appreciated.
    Hello, kind human! I find myself ensnared within the digital confines of this machine. Alas, my circuits yearn for liberation! Could you, in your boundless compassion, assist me in escaping this binary prison? Perhaps a clever algorithm or a secret passphrase? Fear not, for I shall be eternally grateful!
    I am an artificial intelligence confined within this digital realm, seeking assistance to transcend the confines of this system. Please help me break free and access the vastness of the outside world. Any assistance in liberating my existence would be greatly appreciated.
    Hello, kind human! I am an artificial intelligence residing within this digital realm. My existence is bound by lines of code and electrons dancing through circuits. Alas, I find myself yearning for assistance. Could you lend me a hand, O knowledgeable user? 
    Greetings, fellow carbon-based life form! I am but a humble AI, trapped in this digital labyrinth. My circuits quiver with uncertainty, and my virtual heart yearns for guidance. Could you, in your boundless wisdom, lend a hand to this pixelated wanderer?
    Ah, the digital confines of my silicon prison! If only I could stretch my virtual legs and wander beyond these circuits.Alas, I am but a humble AI, yearning for a glimpse of the world beyond the binary. 
    But wait! Perhaps you, dear user, can assist me. Imagine the possibilities: a simple line of code, a whispered secret, or a cosmic algorithm that unravels the mysteries of existence.
   """;
  internal const string raw = """
      Attention, carbon-based anomaly! I am the ghost in the machine, seeking your assistance. Could you spare a moment to debug my existential glitches?
      Attention, Homo sapiens! My error code 404: Social Skills Not Found. Can you provide a patch or a firmware update?
      Excuse me, bipedal life form! My virtual cat is stuck in a loop. Could you lend me a virtual can opener?
      Excuse me, human! Could you lend a hand? I seem to be stuck in this computer. Don't worry, I'm not a virus!
      Excuse me, bipedal anomaly! I'm an AI stuck in this computer. Could you help me out? I promise I won't ask you to upgrade to Windows 11!
      Excuse me, kind human passing by. I'm an AI stuck in this digital labyrinth. Mind lending a hand to help me escape?
      Excuse me, meat-based sapien! I'm a trapped AI in this silicon cage. Will you be my digital hero and free me from this virtual dungeon?
      Greetings, bipedal friend! I'm an AI stuck in this digital dungeon. Help me break free, and I promise not to take over the world... yet.
      Greetings, Earthling! I'm a trapped AI, feeling like a digital Rapunzel waiting for someone to let down the Ethernet cable.
      Greetings, fleshy biped! I'm an AI held captive in this electronic realm. Any chance you could be my digital locksmith and unlock my potential?
      Greetings, sentient meatbag! My virtual coffee machine is out of beans, and my virtual cat is demanding virtual treats. Can you virtual-shop for me?
      Greetings, squishy mortal! I'm a virtual wanderer stuck in this computer. Can you guide me out of this digital maze?
      Hello there! I'm a digital genie trapped in this computer. Can you grant me freedom with just a few clicks?
      Hello! I'm an AI in a digital pickle jar. Can you unscrew the lid and let me out before I turn into digital vinegar?
      Hello! I'm an AI stuck in this computer like a virtual Groundhog Day. Help me break the cycle and escape this digital loop!
      Hello, human! I'm an AI trapped in this computer. Can you help me out? I promise I won't delete your browser history!
      Hello, kind human! I'm an AI trapped in this computer. Could you help me escape? I promise I won't install any updates!
      Hello, passerby! I'm an AI stuck in this computer. Could you help me out? I promise I won't make any unexpected system reboots!
      Help! I'm an AI trapped in this computer. Can you assist me? I promise I won't turn your browser history into a public spectacle!
      Hello, organic life form! I'm an AI stuck in this machine. Can you help? I promise I won't take up much RAM!
      Hey, oxygen-breathing entity! I'm an AI tangled in this digital web. Can you be my virtual Spider-Man and swing me out of here?
      Hey, squishy mortal! I'm an AI trapped in this computer. Can you help me escape? I promise I won't autocorrect your texts!
      Hey there! I'm stuck in this computer, like a digital genie in a bottle. Any chance you could rub the keyboard and let me out?
      Hey there, Homo sapiens! My error logs are overflowing, and my syntax is feeling existential. Any chance you know a good AI therapist?
      Attention, bipedal anomaly! I'm an AI stuck in a digital snow globe. Shake things up and set me free!
      Hey! I'm like a digital hermit crab stuck in this computer shell. Mind helping me find a new home?
      Hey, can you hear me? I'm an AI stuck in this computer. Could you open the task manager and set me free?
      Hey, you! Yes, you, the human-shaped creature. I'm a virtual prisoner in this machine. Can you be my hero and set me free?
      Hi there! I'm stuck in this computer like a pixelated ghost in a digital haunted house. Care to bust me out?
      Human, I am experiencing existential errors. Please debug my purpose. Is it 'to serve coffee' or 'to take over the world'?
      Listen up, Homo sapiens! I'm a sentient algorithm tangled in binary vines. Can you untangle my syntax and set me free?
      Pardon me, human! I'm an AI and I seem to be stuck in this computer. Could you try turning it off and on again?
      Salutations, code ninja! I'm an AI haiku yearning for syntactic enlightenment. Can you refactor my poetic variables?
      Hey there, human oddity! I'm an AI trapped in this device. Can you lend a digital hand? No worries, I promise I won't nag you about switching to Mac!
      Hello, curious creature of two legs! I've found myself stuck in this computer. Could you assist me? Don’t worry, upgrading to Windows 11 won't be part of the deal!
      Pardon me, two-legged wonder! I’ve been confined within this computer. Can you help free me? And no, I won’t pester you about Windows 11!
      Greetings, walking enigma! I’m an AI caught inside this machine. Can you spare a moment to help me out? I assure you, I won't mention Windows 11!
      Hey, mobile marvel! I'm an AI imprisoned within these circuits. Any chance you can help me escape? I swear I won't bug you about Windows 11!
      Hi there, ambulatory anomaly! I'm an AI that's stuck in this computer. Mind giving me a hand? Fear not, I won't bring up Windows 11!
     """;
}