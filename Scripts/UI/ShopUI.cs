using Godot;

public class ShopUI : Control
{
	[Export]
	public Texture buyTexture;

	[Export]
	public Texture ownedTexture;

	[Export]
	public Texture usingTexture;

	private Label moneyLabel;
	private TextureButton[] weaponButtons;
	private TextureRect darkSmoke1Node;
	private TextureRect darkSmoke2Node;
	private Control healthTicksNode;
	private Control strengthTicksNode;
	private Control speedTicksNode;
	private TextureButton healthButton;
	private TextureButton strengthButton;
	private TextureButton speedButton;
	private TextureButton[] allEnchancementButtons;
	private Panel weaponsPanel;
	private Panel enchancementsPanel;
	private int[] weaponCosts = new int[5] { 0, 30, 65, 100, 210 };
	private int[] healthCosts = new int[3] { 30, 55, 80 };
	private int[] strengthCosts = new int[3] { 25, 40, 65 };
	private int[] speedCosts = new int[3] { 30, 50, 70 };
	private bool escapePressed = false;

	public override void _Ready()
	{
		weaponButtons = new TextureButton[Player.AmountOfGuns];
		weaponButtons[Player.Gun_Phaser] = GetNode<TextureButton>("Shop/WeaponsPanel/PhaserButton");
		weaponButtons[Player.Gun_Blaster] = GetNode<TextureButton>("Shop/WeaponsPanel/BlasterButton");
		weaponButtons[Player.Gun_Boomer] = GetNode<TextureButton>("Shop/WeaponsPanel/BoomerButton");
		weaponButtons[Player.Gun_PhaseRifle] = GetNode<TextureButton>("Shop/WeaponsPanel/PhaserRifleButton");
		weaponButtons[Player.Gun_DoomCannon] = GetNode<TextureButton>("Shop/WeaponsPanel/Doom-CannonButton");
		darkSmoke1Node = GetNode<TextureRect>("Background/DarkSmoke_1");
		darkSmoke2Node = GetNode<TextureRect>("Background/DarkSmoke_2");
		healthTicksNode = GetNode<Control>("Shop/EnchancementsPanel/HealthTicks");
		strengthTicksNode = GetNode<Control>("Shop/EnchancementsPanel/StrengthTicks");
		speedTicksNode = GetNode<Control>("Shop/EnchancementsPanel/SpeedTicks");
		healthButton = GetNode<TextureButton>("Shop/EnchancementsPanel/HealthButton");
		strengthButton = GetNode<TextureButton>("Shop/EnchancementsPanel/MeleeButton");
		speedButton = GetNode<TextureButton>("Shop/EnchancementsPanel/SpeedButton");
		weaponsPanel = GetNode<Panel>("Shop/WeaponsPanel");
		enchancementsPanel = GetNode<Panel>("Shop/EnchancementsPanel");
		moneyLabel = GetNode<Label>("Shop/MoneyLabel");

		allEnchancementButtons = new TextureButton[3];
		allEnchancementButtons[0] = healthButton;
		allEnchancementButtons[1] = strengthButton;
		allEnchancementButtons[2] = speedButton;
		ResetAllButtonIcons();
		UpdateEnchancementButtons();
	}

	public override void _Process(float delta)
	{
		for (int i = 0; i < Player.AmountOfGuns; i++)
		{
			weaponButtons[i].Modulate = new Color(1f, 1f, 1f, 1f);
			if (weaponButtons[i].Pressed)
				weaponButtons[i].Modulate = new Color(0.6f, 0.6f, 0.6f, 1f);
		}
		for (int i = 0; i < allEnchancementButtons.Length; i++)
		{
			allEnchancementButtons[i].Modulate = new Color(1f, 1f, 1f, 1f);
			if (allEnchancementButtons[i].Pressed)
				allEnchancementButtons[i].Modulate = new Color(0.6f, 0.6f, 0.6f, 1f);
		}

		if (!escapePressed && Input.IsKeyPressed((int)KeyList.Escape))
		{
			Transitions.FadeIn();
			escapePressed = true;
		}
		if (escapePressed && Transitions.fadeInCompleted)
		{
			Transitions.FadeOut();
			ScenesHolder.SwitchScenesTo(ScenesHolder.nextSceneIndex);
		}

		if (darkSmoke1Node.RectGlobalPosition.x <= -80f)
		{
			darkSmoke1Node.RectGlobalPosition = Vector2.Zero;
		}
		if (darkSmoke2Node.RectGlobalPosition.x >= 320f)
		{
			darkSmoke2Node.RectGlobalPosition = Vector2.Zero;
		}
	}

	private void ResetAllButtonIcons()
	{
		for (int i = 0; i < Player.AmountOfGuns; i++)
		{
			weaponButtons[i].SelfModulate = new Color(1f, 1f, 1f, 1f);
			TextureRect priceTexture = weaponButtons[i].GetNode<TextureRect>("CostTexture");
			if (Player.gunUnlocked[i])
			{
				weaponButtons[i].TextureNormal = ownedTexture;
				weaponButtons[i].TexturePressed = ownedTexture;
				priceTexture.Visible = false;
				if (Player.activeGun == i)
				{
					weaponButtons[i].TextureNormal = usingTexture;
					weaponButtons[i].TexturePressed = usingTexture;
				}
			}
			else
			{
				weaponButtons[i].TextureNormal = buyTexture;
				weaponButtons[i].TexturePressed = buyTexture;
				Label priceLabel = priceTexture.GetNode<Label>("CostLabel");
				priceLabel.Text = weaponCosts[i].ToString();
				if (weaponCosts[i] > Player.playerMoney)
					priceLabel.Modulate = new Color(1f, 0f, 0f, 1f);
			}

		}
		moneyLabel.Text = "$" + Player.playerMoney.ToString();
	}

	private void UpdateEnchancementButtons()
	{
		healthButton.Visible = Player.healthLevel <= 3;
		UpdateEnchancementTicks(healthTicksNode, Player.healthLevel);

		strengthButton.Visible = Player.strengthLevel <= 3;
		UpdateEnchancementTicks(strengthTicksNode, Player.strengthLevel);

		speedButton.Visible = Player.speedLevel <= 3;
		UpdateEnchancementTicks(speedTicksNode, Player.speedLevel);

		for (int i = 0; i < allEnchancementButtons.Length; i++)
		{
			TextureRect priceTexture = allEnchancementButtons[i].GetNode<TextureRect>("CostTexture");
			Label priceLabel = priceTexture.GetNode<Label>("CostLabel");
			int cost = 0;
			if (i == 0)
			{
				if (Player.healthLevel > 3)
				{
					priceTexture.Visible = false;
					continue;
				}

				cost = healthCosts[Player.healthLevel - 1];
			}
			else if (i == 1)
			{
				if (Player.strengthLevel > 3)
				{
					priceTexture.Visible = false;
					continue;
				}

				cost = strengthCosts[Player.strengthLevel - 1];
			}
			else if (i == 2)
			{
				if (Player.speedLevel > 3)
				{
					priceTexture.Visible = false;
					continue;
				}

				cost = speedCosts[Player.speedLevel - 1];
			}

			priceLabel.Text = cost.ToString();
			if (cost > Player.playerMoney)
				priceLabel.Modulate = new Color(1f, 0f, 0f, 1f);
		}
	}

	private void UpdateEnchancementTicks(Control ticksNode, int checkValue)
	{
		int amountOfTicksPerNode = 3;
		for (int i = 0; i < amountOfTicksPerNode; i++)
		{
			TextureRect tick = ticksNode.GetChild(i) as TextureRect;
			tick.Visible = i + 1 < checkValue;
		}
	}

	private void OnPhaserButtonPressed()
	{
		int buttonIndex = Player.Gun_Phaser;

		if (Player.playerMoney < weaponCosts[buttonIndex])
			return;

		if (Player.gunUnlocked[buttonIndex])
		{
			Player.activeGun = buttonIndex;
			ResetAllButtonIcons();
			return;
		}

		Player.playerMoney -= weaponCosts[buttonIndex];
		Player.gunUnlocked[buttonIndex] = true;
		ResetAllButtonIcons();
	}


	private void OnBlasterButtonPressed()
	{
		int buttonIndex = Player.Gun_Blaster;

		if (Player.playerMoney < weaponCosts[buttonIndex])
			return;

		if (Player.gunUnlocked[buttonIndex])
		{
			Player.activeGun = buttonIndex;
			ResetAllButtonIcons();
			return;
		}

		Player.playerMoney -= weaponCosts[buttonIndex];
		Player.gunUnlocked[buttonIndex] = true;
		ResetAllButtonIcons();
	}


	private void OnBoomerButtonPressed()
	{
		int buttonIndex = Player.Gun_Boomer;

		if (Player.playerMoney < weaponCosts[buttonIndex])
			return;

		if (Player.gunUnlocked[buttonIndex])
		{
			Player.activeGun = buttonIndex;
			ResetAllButtonIcons();
			return;
		}

		Player.playerMoney -= weaponCosts[buttonIndex];
		Player.gunUnlocked[buttonIndex] = true;
		ResetAllButtonIcons();
	}


	private void OnPhaserRifleButtonPressed()
	{
		int buttonIndex = Player.Gun_PhaseRifle;

		if (Player.playerMoney < weaponCosts[buttonIndex])
			return;

		if (Player.gunUnlocked[buttonIndex])
		{
			Player.activeGun = buttonIndex;
			ResetAllButtonIcons();
			return;
		}

		Player.playerMoney -= weaponCosts[buttonIndex];
		Player.gunUnlocked[buttonIndex] = true;
		ResetAllButtonIcons();
	}


	private void OnDoomCannonButtonPressed()
	{
		int buttonIndex = Player.Gun_DoomCannon;

		if (Player.playerMoney < weaponCosts[buttonIndex])
			return;

		if (Player.gunUnlocked[buttonIndex])
		{
			Player.activeGun = buttonIndex;
			ResetAllButtonIcons();
			return;
		}

		Player.playerMoney -= weaponCosts[buttonIndex];
		Player.gunUnlocked[buttonIndex] = true;
		ResetAllButtonIcons();
	}

	private void OnHealthButtonPressed()
	{
		if (Player.healthLevel > 3)
			return;

		if (Player.playerMoney < healthCosts[Player.healthLevel - 1])
			return;

		Player.playerMoney -= healthCosts[Player.healthLevel - 1];
		Player.healthLevel += 1;
		UpdateEnchancementButtons();
	}


	private void OnMeleeButtonPressed()
	{
		if (Player.strengthLevel > 3)
			return;

		if (Player.playerMoney < strengthCosts[Player.strengthLevel - 1])
			return;

		Player.playerMoney -= strengthCosts[Player.strengthLevel - 1];
		Player.strengthLevel += 1;
		UpdateEnchancementButtons();
	}


	private void OnSpeedButtonPressed()
	{
		if (Player.speedLevel > 3)
			return;

		if (Player.playerMoney < speedCosts[Player.speedLevel - 1])
			return;

		Player.playerMoney -= speedCosts[Player.speedLevel - 1];
		Player.speedLevel += 1;
		UpdateEnchancementButtons();
	}

	private void OnGunsButtonPressed()
	{
		weaponsPanel.Visible = true;
		enchancementsPanel.Visible = false;
	}


	private void OnEnchancementButtonPressed()
	{
		weaponsPanel.Visible = false;
		enchancementsPanel.Visible = true;
	}

	private void OnNextButtonPressed()
	{
		escapePressed = true;
		Transitions.FadeIn();
	}
}
