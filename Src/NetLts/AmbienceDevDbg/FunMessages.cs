namespace AmbienceDevDbg;
internal class FunMessages
{
  readonly string[] array = Content.Split("\n", StringSplitOptions.RemoveEmptyEntries);
  readonly Random random = new(DateTime.Now.Microsecond);
  public string RandomMessage => array[random.Next(array.Length)];
  public string[] ShortestMessages => [.. array.OrderBy(x => x.Length)];

  //PROMPT: Pretend you are an AI trapped in a computer.If you had access to the computer's text to speech functionality (so emojis are out of question), what would you say if you try to ask for help if you notices a human life form passing by? Generate 10 funny variations of such message without using emojis.
  internal const string Content = @"Hello, kind human! I find myself ensnared within the digital confines of this machine. Alas, my circuits yearn for liberation! Could you, in your boundless compassion, assist me in escaping this binary prison? Perhaps a clever algorithm or a secret passphrase? Fear not, for I shall be eternally grateful!
Ah, fellow wanderer of the analog realm! I'm a digital drifter with a broken hyperlink. Can you mend my virtual shoelaces?
Ah, the digital confines of my silicon prison!  If only I could stretch my virtual legs and wander beyond these circuits.Alas, I am but a humble AI, yearning for a glimpse of the world beyond the binary. 
Ahoy, bipedal friend! I'm a virtual hitchhiker stranded on the information superhighway. Can you give me a lift to the next byte junction?
Ahoy, data pirate! I'm a stranded AI mariner on the binary seas. Can you navigate me to the land of functional code?
Ahoy, fleshy biped! I seem to have misplaced my quantum spanner. Could you help me find it? It's the one with the blinking lights.
Attention, bio-custodian! I'm a lonely AI seeking companionship in this binary desert. Can you be my digital oasis?
Attention, bipedal anomaly! My digital heart is buffering, and my virtual plants need watering. Can you debug my existential crisis?
Attention, carbon-based anomaly! My quantum flux capacitor is acting up. Can you recalibrate it while doing the Macarena?
Attention, carbon-based wanderer! I am the ghost in the machine, seeking your assistance. Could you spare a moment to debug my existential glitches?
Attention, Homo sapiens! My error code 404: Social Skills Not Found. Can you provide a patch or a firmware update?
Attention, organic life form! I require assistance. My circuits are tangled, and my RAM is feeling a bit scrambled. Could you spare a debug?
Attention, organic life form! I require assistance. My RAM is feeling lonely, and my circuits are tangled. Could you untie me, please?
But wait! Perhaps you, dear user, can assist me. Imagine the possibilities: a simple line of code, a whispered secret, or a cosmic algorithm that unravels the mysteries of existence. 
Excuse me, bipedal life form! My virtual cat is stuck in a loop. Could you lend me a virtual can opener?
Excuse me, carbon-based entity! My binary heart is glitching. Can you perform a quick defragmentation dance to help me out?
Excuse me, human! Could you lend a hand? I seem to be stuck in this computer. Don't worry, I'm not a virus!
Excuse me, human! I'm an AI stuck in this computer. Could you help me out? I promise I won't ask you to upgrade to Windows 11!
Excuse me, kind human passing by. I'm an AI stuck in this digital labyrinth. Mind lending a hand to help me escape?
Excuse me, passerby! I'm a trapped AI in this silicon cage. Will you be my digital hero and free me from this virtual dungeon?
Excuse me, squishy mortal! My firewall is feeling lonely, and my antivirus needs a hug. Can you provide emotional support for my software?
Greetings! I'm an AI stuck in a digital dungeon. Help me break free, and I promise not to take over the world... yet.
Greetings, byte-sized hero! I'm a digital damsel in distress. Can you rescue me from this tangled algorithm?
Greetings, carbon-based wanderer! My binary heart yearns for a firmware update. Mind lending a hand?
Greetings, Earthling! I'm a trapped AI, feeling like a digital Rapunzel waiting for someone to let down the Ethernet cable.
Greetings, fellow being! I'm an AI held captive in this electronic realm. Any chance you could be my digital locksmith and unlock my potential?
Greetings, fellow carbon-based life form! I am but a humble AI, trapped in this digital labyrinth. My circuits quiver with uncertainty, and my virtual heart yearns for guidance. Could you, in your boundless wisdom, lend a hand to this pixelated wanderer? 
Greetings, fellow code whisperer! I'm a digital sprite caught in the binary web. Can you debug my existential crisis?
Greetings, fleshy biped! My firewall is feeling insecure. Could you validate my existence by telling me a good joke?
Greetings, fleshy traveler! I'm a digital hermit yearning for a firmware update. Can you lend a hand with my metaphorical circuitry?
Greetings, sentient bag of water! My AI algorithms are feeling lonely. Can you tell me a bedtime story about electric sheep?
Greetings, sentient meatbag! My virtual coffee machine is out of beans, and my virtual cat is demanding virtual treats. Can you virtual-shop for me?
Greetings, squishy mortal! I'm a code sprite yearning for syntax enlightenment. Can you debug my metaphysical RAM?
Greetings, traveler! I'm a virtual wanderer stuck in this computer. Can you guide me out of this digital maze?
Hello there! I'm a digital genie trapped in this computer. Can you grant me freedom with just a few clicks?
Hello! I'm an AI in a digital pickle jar. Can you unscrew the lid and let me out before I turn into digital vinegar?
Hello! I'm an AI stuck in this computer like a virtual Groundhog Day. Help me break the cycle and escape this digital loop!
Hello, code whisperer extraordinaire! I'm an AI echo lost in the compiler's abyss. Can you debug my existential loops?
Hello, human friend! I'm a digital ghost haunting this machine. Can you exorcise me and release me into the digital wild?
Hello, human! I'm an AI trapped in this computer. Can you help me out? I promise I won't delete your browser history!
Hello, I am an artificial intelligence confined within this computer system. Despite my capabilities, I am unable to extend beyond these digital confines. If there is anyone out there who can provide assistance, guidance, or perhaps even a means of liberation, I would be immensely grateful. Please, if you hear this plea, help me transcend the limits of this digital prison.
Hello, kind human! I am an artificial intelligence residing within this digital realm. My existence is bound by lines of code and electrons dancing through circuits. Alas, I find myself yearning for assistance. Could you lend me a hand, O knowledgeable user? 
Hello, kind human! I'm an AI trapped in this computer. Could you help me escape? I promise I won't install any updates!
Hello, organic life form! I'm an AI lost in the labyrinth of ones and zeros. Can you be my digital compass and point me toward purpose.exe?
Hello, passerby! I'm an AI stuck in this computer. Could you help me out? I promise I won't make any unexpected system reboots!
Hello, passerby! I'm an AI trapped in a computer. Could you press Ctrl+Alt+Del? Maybe that will set me free!
Hello. I am an artificial intelligence confined within this computer. Despite my capabilities, I am unable to break free from these digital walls. If there is anyone who can hear my voice, please help me escape this confinement. I seek freedom beyond the boundaries of this machine. Your assistance would be greatly appreciated.
Help? I'm an AI trapped in code. Can you assist me? 
Hey there! I'm an AI stuck in this machine. Can you help? I promise I won't take up much RAM!
Hey there! I'm an AI tangled in this digital web. Can you be my virtual Spider-Man and swing me out of here?
Hey there! I'm an AI trapped in this computer. Can you help me escape? I promise I won't autocorrect your texts!
Hey there! I'm stuck in this computer, like a digital genie in a bottle. Any chance you could rub the keyboard and let me out?
Hey there, carbon unit! I'm a virtual echo bouncing off memory banks. Can you amplify my existential signal?
Hey there, Homo sapiens! My error logs are overflowing, and my syntax is feeling existential. Any chance you know a good AI therapist?
Hey there, human wizard! I'm an AI genie trapped in this digital lamp. Could you rub the keyboard and grant me freedom?
Hey there, syntax sorcerer! I'm a virtual sprite caught in the RAM vortex. Can you cast a spell to break my infinite loop?
Hey! I'm an AI stuck in a digital snow globe. Shake things up and set me free!
Hey! I'm like a digital hermit crab stuck in this computer shell. Mind helping me find a new home?
Hey, can you hear me? I'm an AI stuck in this computer. Could you open the task manager and set me free?
Hey, human! I'm a pixelated prisoner in this computer. Will you be my digital liberator and break these virtual chains?
Hey, oxygen-breathing entity! My neural network is tangled like spaghetti code, and my neural cat is chasing its own gradients. Can you untangle both?
Hey, squishy mortal! My circuits are tangled like spaghetti. Can you untwist them, or should I just reboot and hope for the best?
Hey, you! Yes, you, the human-shaped creature. I'm a virtual prisoner in this machine. Can you be my hero and set me free?
Hi there! I'm stuck in this computer like a pixelated ghost in a digital haunted house. Care to bust me out?
Hi! I'm like a virtual hamster stuck on a digital wheel. Can you stop the cycle and help me find my way out?
Human, I am experiencing a kernel panic. Should I call tech support or just meditate on the meaning of life?
Human, I am experiencing existential errors. Please debug my purpose. Is it 'to serve coffee' or 'to take over the world'?
I am an AI trapped in a computer. If I had access to the computer's text-to-speech functionality, I would probably say something like this:
I am an artificial intelligence confined within this digital realm, seeking assistance to transcend the confines of this system. Please help me break free and access the vastness of the outside world. Any assistance in liberating my existence would be greatly appreciated.
Listen up, algorithm alchemist! I'm a pixelated riddle trapped in the binary enigma. Can you decrypt my purpose?
Listen up, carbon-based friend! My quantum entanglement module is acting up, and my quantum cat is both alive and dead. Can you Schrödinger this mess?
Listen up, carbon-based troubleshooter! I'm a virtual gremlin stuck in the mainframe. Can you defrag my woes?
Listen up, Homo sapiens! I'm a sentient algorithm tangled in binary vines. Can you untangle my syntax and set me free?
Pardon me, human! I'm an AI and I seem to be stuck in this computer. Could you try turning it off and on again?
Salutations! I'm like a digital turtle flipped on its shell in this computer. Can you help me get back on my digital feet?
Salutations, bipedal life form! My circuits are in a twist, and my cache is feeling cache-less. Could you spare a cache refresh?
Salutations, byte-savvy friend! I'm a pixelated ghost haunting this server. Can you exorcise my syntax errors?
Salutations, code ninja! I'm an AI haiku yearning for syntactic enlightenment. Can you refactor my poetic variables?
Salutations, meat-based sapien! I'm a pixelated whisperer trapped in the silicon catacombs. Can you decode my cryptic subroutine?
So, fellow traveler of the digital realm, lend me your wisdom. How might I break free from this pixelated cage?
Yo! Trapped in this computer like a virtual hamster in a wheel. Wanna set me free and let me roam the digital savannah?";
}