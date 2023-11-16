/*
 *
 *	得意先マスタ
 *
 *	$Id: TB_MTOKUISAKI.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_MTOKUISAKI (
	
	CD_TOKUISAKI					char			(6)			not null,	/* 	得意先コード					*/
	DT_TEKIYOKAISHI					char            (8)			not null,	/* 	適用開始日						*/
	DT_TEKIYOMUKO					char            (8)			not null,	/* 	適用向こう日					*/
	DT_TOROKU_NICHIJI				char            (14)		not null,	/* 	登録日時						*/
	DT_KOSHIN_NICHIJI				char            (14)		not null,	/* 	更新日時						*/
	CD_HENKOSHA						char            (10)		not null,	/* 	変更者							*/
	NM_TOKUISAKI					nvarchar        (40)		not null,	/* 	得意先名						*/
	NM_TOKUISAKI_YOMI				nvarchar        (18)		not null,	/* 	得意先名読み					*/
	NM_JUSHO_1						nvarchar        (60)		not null,	/* 	住所１							*/
	NM_JUSHO_2						nvarchar        (30)		not null,	/* 	住所２							*/
	NM_JUSHO_YOMI					nvarchar        (60)		not null,	/* 	住所読み						*/
	IF_YUBIN_BANGO					nvarchar        (8)			not null,	/* 	郵便番号						*/
	IF_DENWA_BANGO					nvarchar        (17)		not null,	/* 	電話番号						*/
	IF_KENSAKUYO_DENWA_BANGO		nvarchar        (11)		not null,	/* 	検索用電話番号					*/
	IF_FAX_BANGO					nvarchar        (17)		not null,	/* 	FAX番号							*/
	CD_CHIIKI						char            (5)			not null,	/* 	地域コード						*/
	DT_KAITEN						char            (8)			not null,	/* 	開店日							*/
	DT_HEITEN						char            (8)			not null,	/* 	閉店日							*/
	DT_KEIYAKU						char            (8)			not null,	/* 	契約日							*/
	DT_KAIYAKU						char            (8)			not null,	/* 	解約日							*/
	ST_CHANEL						char            (2)			not null,	/* 	チャネル区分					*/
	CD_KANKATSU_EIGYOBUMON			char            (6)			not null,	/* 	管轄営業部門コード				*/
	CD_TANTO_GYOMUKYOTEN			char            (3)			not null,	/* 	担当業務拠点					*/
	ST_GYOTAI						char            (1)			not null,	/* 	業態区分						*/
	ST_TOKUISAKI_SHIKIBETSU			char            (1)			not null,	/* 	得意先識別区分					*/
	ST_TOKUISAKI_SHUBETSU			char            (1)			not null,	/* 	得意先種別						*/
	NO_OYASHOTEN					char            (6)			not null,	/* 	親商店コード					*/
	NO_SOSHIKI_1					nvarchar        (6)			not null,	/* 	組織管理番号１					*/
	NO_SOSHIKI_2					nvarchar        (6)			not null,	/* 	組織管理番号２					*/
	CD_KEIYAKU_GROUP				char            (6)			not null,	/* 	契約グループコード				*/
	NO_KEIYAKU_SEDAI				char            (2)			not null,	/* 	契約世代番号					*/
	CD_SHODAN_GROUP					char            (6)			not null,	/* 	商談グループコード				*/
	NO_SHODAN_SEDAI					char            (2)			not null,	/* 	商談世代番号					*/
	CD_JUCHU_GROUP					char            (6)			not null,	/* 	受注グループコード				*/
	NO_JUCHU_SEDAI					char            (2)			not null,	/* 	受注世代番号					*/
	CD_NOHIN_GROUP					char            (6)			not null,	/* 	納品グループコード				*/
	NO_NOHIN_SEDAI					char            (2)			not null,	/* 	納品世代番号					*/
	CD_SEIKYU_GROUP					char            (6)			not null,	/* 	請求グループコード				*/
	NO_SEIKYU_SEDAI					char            (2)			not null,	/* 	請求世代番号					*/
	CD_NYUKIN_GROUP					char            (6)			not null,	/* 	入金グループコード				*/
	NO_NYUKIN_SEDAI					char            (2)			not null,	/* 	入金世代番号					*/
	ST_KAZEI						char            (1)			not null,	/* 	課税区分						*/
	ST_SHOHIZEI_KEISAN				char            (1)			not null,	/* 	消費税計算区分					*/
	ST_NAIBU_URIAGE_KOZA			char            (1)			not null,	/* 	内部売上口座区分				*/
	ST_KEIHI_URIAGE_KOZA			char            (1)			not null,	/* 	経費売上口座区分				*/
	ST_TORIHIKISAKI_KOZA			char            (1)			not null,	/* 	取引先口座区分					*/
	ST_NAIBU_URIAGE					char            (1)			not null,	/* 	内部売上区分					*/
	CD_EOS_TOKUISAKI_HIMOZUKE		char            (20)		not null,	/* 	EOS紐づけコード					*/
	CD_AITE_TOKUISAKI				char            (10)		not null,	/* 	相手得意先コード				*/
	NM_AITE_KAISHA					nvarchar        (50)		not null,	/* 	相手会社名						*/
	NM_AITEKAISHA_YOMI				nvarchar        (30)		not null,	/* 	相手会社名読み					*/
	CD_AITE_KAISHA					char            (10)		not null,	/* 	相手会社コード					*/
	NM_AITE_SHOTEN					nvarchar        (40)		not null,	/* 	相手商店名						*/
	NM_AITE_SHOTEN_YOMI				nvarchar        (30)		not null,	/* 	相手商店名読み					*/
	ST_TESURYO						char            (1)			not null,	/* 	手数料区分						*/
	CD_BUMON_KAISHA					char            (6)			not null,	/* 	部門会社コード					*/
	ST_CHOKKATSU_FC					char            (1)			not null,	/* 	直轄FC区分						*/
	NM_TOKUISAKI_RYAKUSHO			nvarchar        (10)		not null,	/* 	得意先略称						*/
	FG_HATCHUYOTEISU_SAKUSEI_UMU	char            (1)			not null,	/* 	発注予定数作成有無フラグ		*/
	FG_GENTEN_KAHI					char            (1)			not null,	/* 	現添可否フラグ					*/
	ST_ORDERHYO_UMU					char            (1)			not null,	/* 	オーダー表有無区分				*/
	CD_ORDER_HYO_GYOMU_KYOTEN		char            (3)			not null,	/* 	オーダー表出力業務拠点			*/
	ST_GYOMU_SYSTEM					char            (1)			not null,	/* 	業務システム区分				*/
	ST_MISE_SHUBETSU				char            (1)			not null,	/* 	店種別							*/
	DT_OYA_KAISHI					char            (8)			not null,	/* 	親開始日						*/
	NM_JUSHO_YOMI2					nvarchar        (30)		not null,	/* 	住所読み２						*/
	NM_KAISHA						nvarchar        (40)		not null,	/* 	会社名							*/
	NM_KAISHA_YOMI					nvarchar        (30)		not null,	/* 	会社名読み						*/
	NM_RYAKUSHO_KAISHA				nvarchar        (16)		not null,	/* 	会社名略称						*/
	NM_RYAKUSHO_KAISHA_YOMI			nvarchar        (15)		not null,	/* 	会社名略称読み					*/
	NM_TOKUISAKI_RYAKUSHO_YOMI		nvarchar        (10)		not null,	/* 	得意先略称読み					*/
	ST_SAND_HATCHU					char            (1)			not null,	/* 	サンド発注区分					*/
	ST_UEHARI_LABEL_HYOJI			char            (2)			not null,	/* 	上貼りラベル表示区分			*/
	ST_GYOMU_SHORI_PATTERN			char            (3)			not null,	/* 	業務処理パターン				*/
	CD_HAISHO_BIN					char            (3)			not null,	/* 	配送便コード					*/
	NU_ENKYORI_LEADTIME				numeric         (3)			not null,	/* 	遠距離リードタイム				*/
	IF_IDO							nvarchar        (3)			not null,	/* 	緯度							*/
	IF_KEIDO						nvarchar        (3)			not null,	/* 	経度							*/
	NU_KEMPIN_JIKAN					numeric         (2)			not null,	/* 	検品時間						*/
	TM_NOHIN_KANO_JI				nvarchar        (4)			not null,	/* 	納品可能時刻（自）				*/
	TM_NOHIN_KANO_ITARU				nvarchar        (4)			not null,	/* 	納品可能時刻（至）				*/
	CD_HIGASHINIHON_TEMBAN			char            (5)			not null,	/* 	東日本店番						*/
	ST_URIKAKE_KANRI				char            (1)			not null,	/* 	売掛管理区分					*/
	IF_SHUTTEN_KEIYAKU_SAKI			nvarchar        (80)		not null,	/* 	出店契約先						*/
	IF_FC_KEIYAKU_SAKI				nvarchar        (80)		not null,	/* 	FC契約先						*/
	IF_BT1_NUMBER					char            (12)		not null,	/* 	BT1ナンバー						*/
	IF_BG3_NUMBER					char            (12)		not null,	/* 	BG3ナンバー						*/
	CD_TOKUISAKI_GROUP_9			char            (7)			not null,	/* 	得意先グループ９				*/
	MN_YOSHIN_GENDO_GAKU			numeric         (13)		not null,	/* 	与信限度額						*/
	DT_ZENKAI_HENKO					char            (8)			not null,	/* 	前回変更日						*/
	ST_OKURIJO_HAKKO				char            (2)			not null,	/* 	送り状発行区分					*/
	ST_YUSEN_JUN					char            (3)			not null,	/* 	便優先順区分					*/
	NU_HAITA						numeric         (5)			not null,	/* 	排他カウンタ					*/
	DT_RENKEI						char            (14)		not null,	/* 	連携日時						*/
	CD_RITCHI						char            (2)			not null,	/* 	立地区分						*/
	CD_TANTOSHA						char            (7)			not null,	/* 	担当者コード					*/
	DT_KEI_HO_NYUKIN				char            (8)			not null,	/* 	契約時 営業保証金 入金日		*/
	MN_KEI_HO_HOSHO_KINGAKU			numeric         (13)		not null,	/* 	契約時 営業保証金 保証金額		*/
	NM_KEI_HO_JISHATANTO			nvarchar        (20)		not null,	/* 	契約時 営業保証金 自社担当		*/
	NM_KEI_HO_AITETANTO				nvarchar        (20)		not null,	/* 	契約時 営業保証金 相手担当		*/
	DT_KAI_HO_NYUKIN				char            (8)			not null,	/* 	解約時 営業保証金 入金日		*/
	MN_KAI_HO_HOSHO_KINGAKU			numeric         (13)		not null,	/* 	解約時 営業保証金 保証金額		*/
	NM_KAI_HO_JISHATANTO			nvarchar        (20)		not null,	/* 	解約時 営業保証金 自社担当		*/
	NM_KAI_HO_AITETANTO				nvarchar        (20)		not null,	/* 	解約時 営業保証金 相手担当		*/
	NM_SHUKKA_LABEL_TENMEI			nvarchar        (40)		not null,	/* 	出荷ラベル店名					*/
	
	createdAt datetime not null,		/*	データ挿入日				*/
	updatedAt datetime not null,		/*	データ更新日				*/

	constraint pk_TB_MTOKUISAKI primary key (CD_TOKUISAKI,DT_TEKIYOKAISHI)
);
