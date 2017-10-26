﻿using ElectronicObserver.Utility.Data;
using ElectronicObserver.Utility.Mathematics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data
{


	/// <summary>
	/// 個別の艦娘データを保持します。
	/// </summary>
	[DebuggerDisplay("[{ID}] {KCDatabase.Instance.MasterShips[ShipID].NameWithClass} Lv. {Level}")]
	public class ShipData : APIWrapper, IIdentifiable
	{



		/// <summary>
		/// 艦娘を一意に識別するID
		/// </summary>
		public int MasterID => (int)RawData.api_id;

		/// <summary>
		/// 並べ替えの順番
		/// </summary>
		public int SortID => (int)RawData.api_sortno;

		/// <summary>
		/// 艦船ID
		/// </summary>
		public int ShipID => (int)RawData.api_ship_id;

		/// <summary>
		/// レベル
		/// </summary>
		public int Level => (int)RawData.api_lv;

		/// <summary>
		/// 累積経験値
		/// </summary>
		public int ExpTotal => (int)RawData.api_exp[0];

		/// <summary>
		/// 次のレベルに達するために必要な経験値
		/// </summary>
		public int ExpNext => (int)RawData.api_exp[1];


		/// <summary>
		/// 耐久現在値
		/// </summary>
		public int HPCurrent { get; internal set; }

		/// <summary>
		/// 耐久最大値
		/// </summary>
		public int HPMax => (int)RawData.api_maxhp;


		/// <summary>
		/// 速力
		/// </summary>
		public int Speed => RawData.api_soku() ? (int)RawData.api_soku : MasterShip.Speed;

		/// <summary>
		/// 射程
		/// </summary>
		public int Range => (int)RawData.api_leng;


		private int[] _slot;
		/// <summary>
		/// 装備スロット(ID)
		/// </summary>
		public ReadOnlyCollection<int> Slot => Array.AsReadOnly<int>(_slot);


		/// <summary>
		/// 装備スロット(マスターID)
		/// </summary>
		public ReadOnlyCollection<int> SlotMaster
		{
			get
			{
				if (_slot == null) return null;

				int[] s = new int[_slot.Length];

				for (int i = 0; i < s.Length; i++)
				{
					EquipmentData eq = KCDatabase.Instance.Equipments[_slot[i]];
					if (eq != null)
						s[i] = eq.EquipmentID;
					else
						s[i] = -1;
				}

				return Array.AsReadOnly<int>(s);
			}
		}

		/// <summary>
		/// 装備スロット(装備データ)
		/// </summary>
		public ReadOnlyCollection<EquipmentData> SlotInstance
		{
			get
			{
				if (_slot == null) return null;

				var s = new EquipmentData[_slot.Length];

				for (int i = 0; i < s.Length; i++)
				{
					s[i] = KCDatabase.Instance.Equipments[_slot[i]];
				}

				return Array.AsReadOnly(s);
			}
		}

		/// <summary>
		/// 装備スロット(装備マスターデータ)
		/// </summary>
		public ReadOnlyCollection<EquipmentDataMaster> SlotInstanceMaster
		{
			get
			{
				if (_slot == null) return null;

				var s = new EquipmentDataMaster[_slot.Length];

				for (int i = 0; i < s.Length; i++)
				{
					EquipmentData eq = KCDatabase.Instance.Equipments[_slot[i]];
					s[i] = eq != null ? eq.MasterEquipment : null;
				}

				return Array.AsReadOnly(s);
			}
		}


		/// <summary>
		/// 補強装備スロット(ID)
		/// 0=未開放, -1=装備なし 
		/// </summary>
		public int ExpansionSlot { get; private set; }

		/// <summary>
		/// 補強装備スロット(マスターID)
		/// </summary>
		public int ExpansionSlotMaster
		{
			get
			{
				if (ExpansionSlot == 0)
					return 0;

				EquipmentData eq = KCDatabase.Instance.Equipments[ExpansionSlot];
				if (eq != null)
					return eq.EquipmentID;
				else
					return -1;
			}
		}

		/// <summary>
		/// 補強装備スロット(装備データ)
		/// </summary>
		public EquipmentData ExpansionSlotInstance => KCDatabase.Instance.Equipments[ExpansionSlot];

		/// <summary>
		/// 補強装備スロット(装備マスターデータ)
		/// </summary>
		public EquipmentDataMaster ExpansionSlotInstanceMaster
		{
			get
			{
				EquipmentData eq = ExpansionSlotInstance;
				return eq != null ? eq.MasterEquipment : null;
			}
		}


		/// <summary>
		/// 全てのスロット(ID)
		/// </summary>
		public ReadOnlyCollection<int> AllSlot
		{
			get
			{
				if (_slot == null) return null;

				int[] ret = new int[_slot.Length + 1];
				Array.Copy(_slot, ret, _slot.Length);
				ret[ret.Length - 1] = ExpansionSlot;
				return Array.AsReadOnly(ret);
			}
		}

		/// <summary>
		/// 全てのスロット(マスターID)
		/// </summary>
		public ReadOnlyCollection<int> AllSlotMaster
		{
			get
			{
				if (_slot == null) return null;

				var alls = AllSlot;
				int[] ret = new int[alls.Count];
				for (int i = 0; i < ret.Length; i++)
				{
					var eq = KCDatabase.Instance.Equipments[alls[i]];
					if (eq != null) ret[i] = eq.EquipmentID;
					else ret[i] = -1;
				}

				return Array.AsReadOnly(ret);
			}
		}

		/// <summary>
		/// 全てのスロット(装備データ)
		/// </summary>
		public ReadOnlyCollection<EquipmentData> AllSlotInstance
		{
			get
			{
				if (_slot == null) return null;

				var alls = AllSlot;
				EquipmentData[] s = new EquipmentData[alls.Count];

				for (int i = 0; i < s.Length; i++)
				{
					s[i] = KCDatabase.Instance.Equipments[alls[i]];
				}

				return Array.AsReadOnly(s);
			}
		}

		/// <summary>
		/// 全てのスロット(装備マスターデータ)
		/// </summary>
		public ReadOnlyCollection<EquipmentDataMaster> AllSlotInstanceMaster
		{
			get
			{
				if (_slot == null) return null;

				var alls = AllSlot;
				var s = new EquipmentDataMaster[alls.Count];

				for (int i = 0; i < s.Length; i++)
				{
					EquipmentData eq = KCDatabase.Instance.Equipments[alls[i]];
					s[i] = eq != null ? eq.MasterEquipment : null;
				}

				return Array.AsReadOnly(s);
			}
		}



		private int[] _aircraft;
		/// <summary>
		/// 各スロットの航空機搭載量
		/// </summary>
		public ReadOnlyCollection<int> Aircraft => Array.AsReadOnly(_aircraft);


		/// <summary>
		/// 現在の航空機搭載量
		/// </summary>
		public int AircraftTotal => _aircraft.Sum(a => Math.Max(a, 0));


		/// <summary>
		/// 搭載燃料
		/// </summary>
		public int Fuel { get; internal set; }

		/// <summary>
		/// 搭載弾薬
		/// </summary>
		public int Ammo { get; internal set; }


		/// <summary>
		/// スロットのサイズ
		/// </summary>
		public int SlotSize => !RawData.api_slotnum() ? 0 : (int)RawData.api_slotnum;

		/// <summary>
		/// 入渠にかかる時間(ミリ秒)
		/// </summary>
		public int RepairTime => (int)RawData.api_ndock_time;

		/// <summary>
		/// 入渠にかかる鋼材
		/// </summary>
		public int RepairSteel => (int)RawData.api_ndock_item[1];

		/// <summary>
		/// 入渠にかかる燃料
		/// </summary>
		public int RepairFuel => (int)RawData.api_ndock_item[0];

		/// <summary>
		/// コンディション
		/// </summary>
		public int Condition { get; internal set; }


		#region Parameters

		/********************************************************
		 * 強化値：近代化改修・レベルアップによって上昇した数値
		 * 総合値：装備込みでのパラメータ
		 * 基本値：装備なしでのパラメータ(初期値+強化値)
		 ********************************************************/

		private int[] _modernized;
		/// <summary>
		/// 火力強化値
		/// </summary>
		public int FirepowerModernized => _modernized.Length >= 5 ? _modernized[0] : 0;

		/// <summary>
		/// 雷装強化値
		/// </summary>
		public int TorpedoModernized => _modernized.Length >= 5 ? _modernized[1] : 0;

		/// <summary>
		/// 対空強化値
		/// </summary>
		public int AAModernized => _modernized.Length >= 5 ? _modernized[2] : 0;

		/// <summary>
		/// 装甲強化値
		/// </summary>
		public int ArmorModernized => _modernized.Length >= 5 ? _modernized[3] : 0;

		/// <summary>
		/// 運強化値
		/// </summary>
		public int LuckModernized => _modernized.Length >= 5 ? _modernized[4] : 0;

		/// <summary>
		/// 耐久強化値
		/// </summary>
		public int HPMaxModernized => _modernized.Length >= 7 ? _modernized[5] : 0;

		/// <summary>
		/// 対潜強化値
		/// </summary>
		public int ASWModernized => _modernized.Length >= 7 ? _modernized[6] : 0;


		/// <summary>
		/// 火力改修残り
		/// </summary>
		public int FirepowerRemain => (MasterShip.FirepowerMax - MasterShip.FirepowerMin) - FirepowerModernized;

		/// <summary>
		/// 雷装改修残り
		/// </summary>
		public int TorpedoRemain => (MasterShip.TorpedoMax - MasterShip.TorpedoMin) - TorpedoModernized;

		/// <summary>
		/// 対空改修残り
		/// </summary>
		public int AARemain => (MasterShip.AAMax - MasterShip.AAMin) - AAModernized;

		/// <summary>
		/// 装甲改修残り
		/// </summary>
		public int ArmorRemain => (MasterShip.ArmorMax - MasterShip.ArmorMin) - ArmorModernized;

		/// <summary>
		/// 運改修残り
		/// </summary>
		public int LuckRemain => (MasterShip.LuckMax - MasterShip.LuckMin) - LuckModernized;

		/// <summary>
		/// 耐久改修残り
		/// </summary>
		public int HPMaxRemain => (IsMarried ? MasterShip.HPMaxMarriedModernizable : MasterShip.HPMaxModernizable) - HPMaxModernized;

		/// <summary>
		/// 対潜改修残り
		/// </summary>
		public int ASWRemain => ASWMax <= 0 ? 0 : MasterShip.ASWModernizable - ASWModernized;


		/// <summary>
		/// 火力総合値
		/// </summary>
		public int FirepowerTotal => (int)RawData.api_karyoku[0];

		/// <summary>
		/// 雷装総合値
		/// </summary>
		public int TorpedoTotal => (int)RawData.api_raisou[0];

		/// <summary>
		/// 対空総合値
		/// </summary>
		public int AATotal => (int)RawData.api_taiku[0];

		/// <summary>
		/// 装甲総合値
		/// </summary>
		public int ArmorTotal => (int)RawData.api_soukou[0];

		/// <summary>
		/// 回避総合値
		/// </summary>
		public int EvasionTotal => (int)RawData.api_kaihi[0];

		/// <summary>
		/// 対潜総合値
		/// </summary>
		public int ASWTotal => (int)RawData.api_taisen[0];

		/// <summary>
		/// 索敵総合値
		/// </summary>
		public int LOSTotal => (int)RawData.api_sakuteki[0];

		/// <summary>
		/// 運総合値
		/// </summary>
		public int LuckTotal => (int)RawData.api_lucky[0];

		/// <summary>
		/// 爆装総合値
		/// </summary>
		public int BomberTotal => AllSlotInstanceMaster.Sum(s => s == null ? 0 : Math.Max(s.Bomber, 0));


		/// <summary>
		/// 火力基本値
		/// </summary>
		public int FirepowerBase => MasterShip.FirepowerMin + FirepowerModernized;


		/// <summary>
		/// 雷装基本値
		/// </summary>
		public int TorpedoBase => MasterShip.TorpedoMin + TorpedoModernized;


		/// <summary>
		/// 対空基本値
		/// </summary>
		public int AABase => MasterShip.AAMin + AAModernized;


		/// <summary>
		/// 装甲基本値
		/// </summary>
		public int ArmorBase => MasterShip.ArmorMin + ArmorModernized;


		/// <summary>
		/// 回避基本値
		/// </summary>
		public int EvasionBase
		{
			get
			{
				int param = EvasionTotal;
				foreach (var eq in AllSlotInstance)
				{
					if (eq != null)
						param -= eq.MasterEquipment.Evasion;
				}
				return param;
			}
		}

		/// <summary>
		/// 対潜基本値
		/// </summary>
		public int ASWBase
		{
			get
			{
				int param = ASWTotal;
				foreach (var eq in AllSlotInstance)
				{
					if (eq != null)
						param -= eq.MasterEquipment.ASW;
				}
				return param;
			}
		}

		/// <summary>
		/// 索敵基本値
		/// </summary>
		public int LOSBase
		{
			get
			{
				int param = LOSTotal;
				foreach (var eq in AllSlotInstance)
				{
					if (eq != null)
						param -= eq.MasterEquipment.LOS;
				}
				return param;
			}
		}

		/// <summary>
		/// 運基本値
		/// </summary>
		public int LuckBase => MasterShip.LuckMin + LuckModernized;



		/// <summary>
		/// 回避最大値
		/// </summary>
		public int EvasionMax => (int)RawData.api_kaihi[1];

		/// <summary>
		/// 対潜最大値
		/// </summary>
		public int ASWMax => (int)RawData.api_taisen[1];

		/// <summary>
		/// 索敵最大値
		/// </summary>
		public int LOSMax => (int)RawData.api_sakuteki[1];

		#endregion


		/// <summary>
		/// 保護ロックの有無
		/// </summary>
		public bool IsLocked => (int)RawData.api_locked != 0;

		/// <summary>
		/// 装備による保護ロックの有無
		/// </summary>
		public bool IsLockedByEquipment => (int)RawData.api_locked_equip != 0;


		/// <summary>
		/// 出撃海域
		/// </summary>
		public int SallyArea => RawData.api_sally_area() ? (int)RawData.api_sally_area : -1;



		/// <summary>
		/// 艦船のマスターデータへの参照
		/// </summary>
		public ShipDataMaster MasterShip => KCDatabase.Instance.MasterShips[ShipID];


		/// <summary>
		/// 入渠中のドックID　非入渠時は-1
		/// </summary>
		public int RepairingDockID
		{
			get
			{
				foreach (var dock in KCDatabase.Instance.Docks.Values)
				{
					if (dock.ShipID == MasterID)
						return dock.DockID;
				}
				return -1;
			}
		}

		/// <summary>
		/// 所属艦隊　-1=なし
		/// </summary>
		public int Fleet
		{
			get
			{
				FleetManager fm = KCDatabase.Instance.Fleet;
				foreach (var f in fm.Fleets.Values)
				{
					if (f.Members.Contains(MasterID))
						return f.FleetID;
				}
				return -1;
			}
		}


		/// <summary>
		/// 所属艦隊及びその位置
		/// ex. 1-3 (位置も1から始まる)
		/// 所属していなければ 空文字列
		/// </summary>
		public string FleetWithIndex
		{
			get
			{
				FleetManager fm = KCDatabase.Instance.Fleet;
				foreach (var f in fm.Fleets.Values)
				{
					int index = f.Members.IndexOf(MasterID);
					if (index != -1)
					{
						return string.Format("{0}-{1}", f.FleetID, index + 1);
					}
				}
				return "";
			}

		}


		/// <summary>
		/// ケッコン済みかどうか
		/// </summary>
		public bool IsMarried => Level > 99;


		/// <summary>
		/// 次の改装まで必要な経験値
		/// </summary>
		public int ExpNextRemodel
		{
			get
			{
				ShipDataMaster master = MasterShip;
				if (master.RemodelAfterShipID <= 0)
					return 0;
				return Math.Max(ExpTable.ShipExp[master.RemodelAfterLevel].Total - ExpTotal, 0);
			}
		}


		/// <summary>
		/// 艦名
		/// </summary>
		public string Name => MasterShip.Name;


		/// <summary>
		/// 艦名(レベルを含む)
		/// </summary>
		public string NameWithLevel => string.Format("{0} Lv. {1}", MasterShip.Name, Level);


		/// <summary>
		/// HP/HPmax
		/// </summary>
		public double HPRate => HPMax > 0 ? (double)HPCurrent / HPMax : 0;



		/// <summary>
		/// 最大搭載燃料
		/// </summary>
		public int FuelMax => MasterShip.Fuel;

		/// <summary>
		/// 最大搭載弾薬
		/// </summary>
		public int AmmoMax => MasterShip.Ammo;


		/// <summary>
		/// 燃料残量割合
		/// </summary>
		public double FuelRate => (double)Fuel / Math.Max(FuelMax, 1);

		/// <summary>
		/// 弾薬残量割合
		/// </summary>
		public double AmmoRate => (double)Ammo / Math.Max(AmmoMax, 1);


		/// <summary>
		/// 搭載機残量割合
		/// </summary>
		public ReadOnlyCollection<double> AircraftRate
		{
			get
			{
				double[] airs = new double[_aircraft.Length];
				var airmax = MasterShip.Aircraft;

				for (int i = 0; i < airs.Length; i++)
				{
					airs[i] = (double)_aircraft[i] / Math.Max(airmax[i], 1);
				}

				return Array.AsReadOnly(airs);
			}
		}

		/// <summary>
		/// 搭載機残量割合
		/// </summary>
		public double AircraftTotalRate => (double)AircraftTotal / Math.Max(MasterShip.AircraftTotal, 1);





		/// <summary>
		/// 補強装備スロットが使用可能か
		/// </summary>
		public bool IsExpansionSlotAvailable => ExpansionSlot != 0;



		#region ダメージ威力計算

		/// <summary>
		/// 航空戦威力
		/// 本来スロットごとのものであるが、ここでは最大火力を採用する
		/// </summary>
		public int AirBattlePower => _airbattlePowers.Max();

		private int[] _airbattlePowers;
		/// <summary>
		/// 各スロットの航空戦威力
		/// </summary>
		public ReadOnlyCollection<int> AirBattlePowers => Array.AsReadOnly(_airbattlePowers);

		/// <summary>
		/// 砲撃威力
		/// </summary>
		public int ShellingPower { get; private set; }

		//todo: ShellingPower に統合予定
		/// <summary>
		/// 空撃威力
		/// </summary>
		public int AircraftPower { get; private set; }

		/// <summary>
		/// 対潜威力
		/// </summary>
		public int AntiSubmarinePower { get; private set; }

		/// <summary>
		/// 雷撃威力
		/// </summary>
		public int TorpedoPower { get; private set; }

		/// <summary>
		/// 夜戦威力
		/// </summary>
		public int NightBattlePower { get; private set; }



		/// <summary>
		/// 装備改修補正(砲撃戦)
		/// </summary>
		private double GetDayBattleEquipmentLevelBonus()
		{

			double basepower = 0;
			foreach (var slot in AllSlotInstance)
			{
				if (slot == null)
					continue;

				switch (slot.MasterEquipment.CategoryType)
				{
					case 3:     // 大口径主砲
					case 38:
						basepower += Math.Sqrt(slot.Level) * 1.5;
						break;
					case 14:    // ソナー
					case 15:    // 爆雷
						basepower += Math.Sqrt(slot.Level) * 0.75;
						break;
					case 5:     // 魚雷
					case 10:    // 水上偵察機
					case 12:    // 小型電探
					case 13:    // 大型電探
					case 32:    // 潜水艦魚雷
						break;  //  → 無視
					default:
						basepower += Math.Sqrt(slot.Level);
						break;
				}
			}
			return basepower;
		}

		/// <summary>
		/// 装備改修補正(空撃)
		/// </summary>
		/// <returns></returns>
		private double GetAircraftEquipmentLevelBonus()
		{

			double basepower = 0;
			foreach (var slot in AllSlotInstance)
			{
				if (slot == null)
					continue;

				switch (slot.MasterEquipment.CategoryType)
				{
					case 4:     // 副砲
						basepower += Math.Sqrt(slot.Level);
						break;
				}
			}
			return basepower;
		}

		/// <summary>
		/// 装備改修補正(雷撃戦)
		/// </summary>
		private double GetTorpedoEquipmentLevelBonus()
		{
			double basepower = 0;
			foreach (var slot in AllSlotInstance)
			{
				if (slot == null)
					continue;

				switch (slot.MasterEquipment.CategoryType)
				{
					case 5:     // 魚雷
					case 21:    // 機銃
					case 32:    // 潜水艦魚雷
						basepower += Math.Sqrt(slot.Level) * 1.2;
						break;
				}
			}
			return basepower;
		}

		/// <summary>
		/// 装備改修補正(対潜)
		/// </summary>
		private double GetAntiSubmarineEquipmentLevelBonus()
		{
			double basepower = 0;
			foreach (var slot in AllSlotInstance)
			{
				if (slot == null)
					continue;

				switch (slot.MasterEquipment.CategoryType)
				{
					case 14:    // 爆雷
					case 15:    // ソナー
						basepower += Math.Sqrt(slot.Level) * 1.2;
						break;
				}
			}
			return basepower;
		}

		/// <summary>
		/// 装備改修補正(夜戦)
		/// </summary>
		private double GetNightBattleEquipmentLevelBonus()
		{
			double basepower = 0;
			foreach (var slot in AllSlotInstance)
			{
				if (slot == null)
					continue;

				switch (slot.MasterEquipment.CategoryType)
				{
					case 1:     // 小口径主砲
					case 2:     // 中口径主砲
					case 3:     // 大口径主砲
					case 4:     // 副砲
					case 5:     // 魚雷
					case 19:    // 徹甲弾
					case 24:    // 上陸用舟艇
					case 29:    // 探照灯
					case 32:    // 潜水艦魚雷
					case 36:    // 高射装置
					case 38:    // 大口径主砲(II)
					case 42:    // 大型探照灯
					case 46:    // 特型内火艇
						basepower += Math.Sqrt(slot.Level);
						break;
				}
			}
			return basepower;
		}

		/// <summary>
		/// 耐久値による攻撃力補正
		/// </summary>
		private double GetHPDamageBonus()
		{
			if (HPRate < 0.25)
				return 0.4;
			else if (HPRate < 0.5)
				return 0.7;
			else
				return 1.0;
		}

		/// <summary>
		/// 交戦形態による威力補正
		/// </summary>
		private double GetEngagementFormDamageRate(int form)
		{
			switch (form)
			{
				case 1:     // 同航戦
				default:
					return 1.0;
				case 2:     // 反航戦
					return 0.8;
				case 3:     // T字有利
					return 1.2;
				case 4:     // T字不利
					return 0.6;
			}
		}

		/// <summary>
		/// 残り弾薬量による威力補正
		/// <returns></returns>
		private double GetAmmoDamageRate()
		{
			return Math.Min(Math.Floor(AmmoRate * 100) / 50.0, 1.0);
		}

		/// <summary>
		/// 連合艦隊編成における砲撃戦火力補正
		/// </summary>
		private double GetCombinedFleetShellingDamageBonus()
		{
			int fleet = Fleet;
			if (fleet == -1 || fleet > 2)
				return 0;

			switch (KCDatabase.Instance.Fleet.CombinedFlag)
			{
				case 1:     //機動部隊
					if (fleet == 1)
						return +2;
					else
						return +10;

				case 2:     //水上部隊
					if (fleet == 1)
						return +10;
					else
						return -5;

				case 3:     //輸送部隊
					if (fleet == 1)
						return -5;
					else
						return +10;

				default:
					return 0;
			}
		}

		/// <summary>
		/// 連合艦隊編成における雷撃戦火力補正
		/// </summary>
		private double GetCombinedFleetTorpedoDamageBonus()
		{
			int fleet = Fleet;
			if (fleet == -1 || fleet > 2)
				return 0;

			if (KCDatabase.Instance.Fleet.CombinedFlag == 0)
				return 0;

			return -5;
		}

		/// <summary>
		/// 軽巡軽量砲補正
		/// </summary>
		private double GetLightCruiserDamageBonus()
		{
			if (MasterShip.ShipType == 3 ||
				MasterShip.ShipType == 4 ||
				MasterShip.ShipType == 21)
			{   //軽巡/雷巡/練巡

				int single = 0;
				int twin = 0;

				foreach (var slot in AllSlotMaster)
				{
					if (slot == -1) continue;

					switch (slot)
					{
						case 4:     //14cm単装砲
						case 11:    //15.2cm単装砲
							single++;
							break;
						case 65:    //15.2cm連装砲
						case 119:   //14cm連装砲
						case 139:   //15.2cm連装砲改
							twin++;
							break;
					}
				}

				return Math.Sqrt(twin) * 2.0 + Math.Sqrt(single);
			}

			return 0;
		}

		/// <summary>
		/// イタリア重巡砲補正
		/// </summary>
		/// <returns></returns>
		private double GetItalianDamageBonus()
		{
			switch (ShipID)
			{
				case 448:       // Zara
				case 358:       // 改
				case 496:       // due
				case 449:       // Pola
				case 361:       // 改
					return Math.Sqrt(AllSlotMaster.Count(id => id == 162));     // √( 203mm/53 連装砲 装備数 )

				default:
					return 0;
			}
		}

		private double CapDamage(double damage, int max)
		{
			if (damage < max)
				return damage;
			else
				return max + Math.Sqrt(damage - max);
		}


		/// <summary>
		/// 航空戦での威力を求めます。
		/// </summary>
		/// <param name="slotIndex">スロットのインデックス。 0 起点です。</param>
		private int CalculateAirBattlePower(int slotIndex)
		{
			double basepower = 0;
			var slots = AllSlotInstance;

			var eq = SlotInstance[slotIndex];

			if (eq == null || _aircraft[slotIndex] == 0)
				return 0;

			switch (eq.MasterEquipment.CategoryType)
			{
				case 7:     //艦爆
				case 11:    //水爆
					basepower = eq.MasterEquipment.Bomber * Math.Sqrt(_aircraft[slotIndex]) + 25;
					break;
				case 8:     //艦攻
							// 150% 補正を引いたとする
					basepower = (eq.MasterEquipment.Torpedo * Math.Sqrt(_aircraft[slotIndex]) + 25) * 1.5;
					break;
				default:
					return 0;
			}

			//キャップ
			basepower = Math.Floor(CapDamage(basepower, 150));

			return (int)(basepower * GetAmmoDamageRate());
		}

		/// <summary>
		/// 砲撃戦での砲撃威力を求めます。
		/// </summary>
		/// <param name="engagementForm">交戦形態。既定値は 1 (同航戦) です。</param>
		private int CalculateShellingPower(int engagementForm = 1)
		{
			var attackKind = Calculator.GetDayAttackKind(AllSlotMaster.ToArray(), ShipID, -1, false);
			if (attackKind == DayAttackKind.AirAttack || attackKind == DayAttackKind.CutinAirAttack)
				return 0;


			double basepower = FirepowerTotal + GetDayBattleEquipmentLevelBonus() + GetCombinedFleetShellingDamageBonus() + 5;

			basepower *= GetHPDamageBonus() * GetEngagementFormDamageRate(engagementForm);

			basepower += GetLightCruiserDamageBonus() + GetItalianDamageBonus();

			//キャップ
			basepower = Math.Floor(CapDamage(basepower, 180));

			//弾着
			switch (Calculator.GetDayAttackKind(AllSlotMaster.ToArray(), ShipID, -1))
			{
				case DayAttackKind.DoubleShelling:
				case DayAttackKind.CutinMainLadar:
					basepower *= 1.2;
					break;
				case DayAttackKind.CutinMainSub:
					basepower *= 1.1;
					break;
				case DayAttackKind.CutinMainAP:
					basepower *= 1.3;
					break;
				case DayAttackKind.CutinMainMain:
					basepower *= 1.5;
					break;
			}

			return (int)(basepower * GetAmmoDamageRate());
		}

		/// <summary>
		/// 砲撃戦での空撃威力を求めます。
		/// </summary>
		/// <param name="engagementForm">交戦形態。既定値は 1 (同航戦) です。</param>
		private int CalculateAircraftPower(int engagementForm = 1)
		{
			var attackKind = Calculator.GetDayAttackKind(AllSlotMaster.ToArray(), ShipID, -1, false);
			if (attackKind != DayAttackKind.AirAttack && attackKind != DayAttackKind.CutinAirAttack)
				return 0;


			double basepower = Math.Floor((FirepowerTotal + TorpedoTotal + Math.Floor(BomberTotal * 1.3) + GetAircraftEquipmentLevelBonus() + GetCombinedFleetShellingDamageBonus()) * 1.5) + 55;

			basepower *= GetHPDamageBonus() * GetEngagementFormDamageRate(engagementForm);

			//キャップ
			basepower = Math.Floor(CapDamage(basepower, 180));

			return (int)(basepower * GetAmmoDamageRate());
		}

		/// <summary>
		/// 砲撃戦での対潜威力を求めます。
		/// </summary>
		/// <param name="engagementForm">交戦形態。既定値は 1 (同航戦) です。</param>
		private int CalculateAntiSubmarinePower(int engagementForm = 1)
		{
			if (!Calculator.CanAttackSubmarine(this))
				return 0;

			double eqpower = 0;
			foreach (var slot in AllSlotInstance)
			{
				if (slot == null)
					continue;

				switch (slot.MasterEquipment.CategoryType)
				{
					case 7:     //艦爆
					case 8:     //艦攻
					case 11:    //水爆
					case 14:    //ソナー
					case 15:    //爆雷
					case 25:    //オートジャイロ
					case 26:    //対潜哨戒機
					case 40:    //大型ソナー
						eqpower += slot.MasterEquipment.ASW;
						break;
				}
			}

			double basepower = Math.Sqrt(ASWBase) * 2 + eqpower * 1.5 + GetAntiSubmarineEquipmentLevelBonus();
			if (Calculator.GetDayAttackKind(AllSlotMaster.ToArray(), ShipID, 126, false) == DayAttackKind.AirAttack)
			{       //126=伊168; 対潜攻撃が空撃なら
				basepower += 8;
			}
			else
			{   //爆雷攻撃なら
				basepower += 13;
			}


			basepower *= GetHPDamageBonus() * GetEngagementFormDamageRate(engagementForm);


			//対潜シナジー

			int depthChargeCount = 0;
			int depthChargeThrowerCount = 0;
			int sonarCount = 0;         // ソナーと大型ソナーの合算
			int largeSonarCount = 0;

			foreach (var slot in AllSlotInstanceMaster)
			{
				if (slot == null)
					continue;

				switch (slot.CategoryType)
				{
					case 14:    // ソナー
						sonarCount++;
						break;
					case 15:    // 爆雷/投射機
						if (Calculator.DepthChargeList.Contains(slot.EquipmentID))
							depthChargeCount++;
						else
							depthChargeThrowerCount++;
						break;
					case 40:    // 大型ソナー
						largeSonarCount++;
						sonarCount++;
						break;
				}
			}

			double thrower_sonar = depthChargeThrowerCount > 0 && sonarCount > 0 ? 1.15 : 1;
			double charge_thrower = depthChargeCount > 0 && depthChargeThrowerCount > 0 ? 1.1 : 1;
			double charge_sonar = (!(thrower_sonar > 1 && charge_thrower > 1 && largeSonarCount > 0) && depthChargeCount > 0 && sonarCount > 0) ? 0.15 : 0;

			basepower *= thrower_sonar * (charge_thrower + charge_sonar);


			//キャップ
			basepower = Math.Floor(CapDamage(basepower, 100));

			return (int)(basepower * GetAmmoDamageRate());
		}

		/// <summary>
		/// 雷撃戦での威力を求めます。
		/// </summary>
		/// <param name="engagementForm">交戦形態。既定値は 1 (同航戦) です。</param>
		private int CalculateTorpedoPower(int engagementForm = 1)
		{
			if (TorpedoBase == 0)
				return 0;       //雷撃不能艦は除外

			double basepower = TorpedoTotal + GetTorpedoEquipmentLevelBonus() + GetCombinedFleetTorpedoDamageBonus() + 5;

			basepower *= GetHPDamageBonus() * GetEngagementFormDamageRate(engagementForm);      //開幕雷撃は中大破補正が違うが見なかったことに

			//キャップ
			basepower = Math.Floor(CapDamage(basepower, 150));


			return (int)(basepower * GetAmmoDamageRate());
		}

		/// <summary>
		/// 夜戦での威力を求めます。
		/// </summary>
		private int CalculateNightBattlePower()
		{
			double basepower = FirepowerTotal + TorpedoTotal + GetNightBattleEquipmentLevelBonus();

			basepower *= GetHPDamageBonus();

			switch (Calculator.GetNightAttackKind(AllSlotMaster.ToArray(), ShipID, -1))
			{
				case NightAttackKind.DoubleShelling:
					basepower *= 1.2;
					break;
				case NightAttackKind.CutinMainTorpedo:
					basepower *= 1.3;
					break;
				case NightAttackKind.CutinTorpedoTorpedo:
					{
						switch (Calculator.GetNightTorpedoCutinKind(AllSlotMaster.ToArray(), ShipID, -1))
						{
							case 1:
								basepower *= 1.75;
								break;
							case 2:
								basepower *= 1.6;
								break;
							default:
								basepower *= 1.5;
								break;
						}
					}
					break;
				case NightAttackKind.CutinMainSub:
					basepower *= 1.75;
					break;
				case NightAttackKind.CutinMainMain:
					basepower *= 2.0;
					break;
			}

			basepower += GetLightCruiserDamageBonus() + GetItalianDamageBonus();

			//キャップ
			basepower = Math.Floor(CapDamage(basepower, 300));


			return (int)(basepower * GetAmmoDamageRate());
		}


		/// <summary>
		/// 威力系の計算をまとめて行い、プロパティを更新します。
		/// </summary>
		private void CalculatePowers()
		{

			int form = Utility.Configuration.Config.Control.PowerEngagementForm;

			_airbattlePowers = _slot.Select((_, i) => CalculateAirBattlePower(i)).ToArray();
			ShellingPower = CalculateShellingPower(form);
			AircraftPower = CalculateAircraftPower(form);
			AntiSubmarinePower = CalculateAntiSubmarinePower(form);
			TorpedoPower = CalculateTorpedoPower(form);
			NightBattlePower = CalculateNightBattlePower();

		}


		#endregion




		public int ID => MasterID;


		public override void LoadFromResponse(string apiname, dynamic data)
		{

			switch (apiname)
			{
				default:
					base.LoadFromResponse(apiname, (object)data);

					HPCurrent = (int)RawData.api_nowhp;
					Fuel = (int)RawData.api_fuel;
					Ammo = (int)RawData.api_bull;
					Condition = (int)RawData.api_cond;
					_slot = (int[])RawData.api_slot;
					ExpansionSlot = (int)RawData.api_slot_ex;
					_aircraft = (int[])RawData.api_onslot;
					_modernized = (int[])RawData.api_kyouka;
					break;

				case "api_req_hokyu/charge":
					Fuel = (int)data.api_fuel;
					Ammo = (int)data.api_bull;
					_aircraft = (int[])data.api_onslot;
					break;

				case "api_req_kaisou/slot_exchange_index":
					_slot = (int[])data.api_slot;
					break;
			}

			CalculatePowers();
		}


		public override void LoadFromRequest(string apiname, Dictionary<string, string> data)
		{
			base.LoadFromRequest(apiname, data);

			KCDatabase db = KCDatabase.Instance;

			switch (apiname)
			{
				case "api_req_kousyou/destroyship":
					{

						for (int i = 0; i < _slot.Length; i++)
						{
							if (_slot[i] == -1) continue;
							db.Equipments.Remove(_slot[i]);
						}
					}
					break;

				case "api_req_kaisou/open_exslot":
					ExpansionSlot = -1;
					break;
			}
		}


		/// <summary>
		/// 入渠完了時の処理を行います。
		/// </summary>
		internal void Repair()
		{

			HPCurrent = HPMax;
			Condition = Math.Max(Condition, 40);

			RawData.api_ndock_time = 0;
			RawData.api_ndock_item[0] = 0;
			RawData.api_ndock_item[1] = 0;

		}

	}

}

