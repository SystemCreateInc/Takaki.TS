/*
 *
 *	得意先マスタ
 *
 *	$Id: TB_MTOKUISAKI.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_MTOKUISAKI (
	
	CD_TOKUISAKI					nchar			(6)			not null,	/* 	得意先コード					*/
	DT_TEKIYOKAISHI					nchar           (8)			not null,	/* 	適用開始日						*/
	DT_TEKIYOMUKO					nchar           (8)			not null,	/* 	適用向こう日					*/
	DT_TOROKU_NICHIJI				nchar           (14)		not null,	/* 	登録日時						*/
	DT_KOSHIN_NICHIJI				nchar           (14)		not null,	/* 	更新日時						*/
	CD_HENKOSHA						nchar           (10)		not null,	/* 	変更者							*/
	NM_TOKUISAKI					nvarchar        (40)		not null,	/* 	得意先名						*/
	NM_TOKUISAKI_YOMI				nvarchar        (18)		not null,	/* 	得意先名読み					*/
	NM_JUSHO_1						nvarchar        (60)		not null,	/* 	住所１							*/
	NM_JUSHO_2						nvarchar        (30)		not null,	/* 	住所２							*/
	NM_JUSHO_YOMI					nvarchar        (60)		not null,	/* 	住所読み						*/
	IF_YUBIN_BANGO					nvarchar        (8)			not null,	/* 	郵便番号						*/
	IF_DENWA_BANGO					nvarchar        (17)		not null,	/* 	電話番号						*/
	IF_KENSAKUYO_DENWA_BANGO		nvarchar        (11)		not null,	/* 	検索用電話番号					*/
	IF_FAX_BANGO					nvarchar        (17)		not null,	/* 	FAX番号							*/
	CD_CHIIKI						nchar           (5)			not null,	/* 	地域コード						*/
	DT_KAITEN						nchar           (8)			not null,	/* 	開店日							*/
	DT_HEITEN						nchar           (8)			not null,	/* 	閉店日							*/
	DT_KEIYAKU						nchar           (8)			not null,	/* 	契約日							*/
	DT_KAIYAKU						nchar           (8)			not null,	/* 	解約日							*/
	ST_CHANEL						nchar           (2)			not null,	/* 	チャネル区分					*/
	CD_KANKATSU_EIGYOBUMON			nchar           (6)			not null,	/* 	管轄営業部門コード				*/
	CD_TANTO_GYOMUKYOTEN			nchar           (3)			not null,	/* 	担当業務拠点					*/
	ST_GYOTAI						nchar           (1)			not null,	/* 	業態区分						*/
	ST_TOKUISAKI_SHIKIBETSU			nchar           (1)			not null,	/* 	得意先識別区分					*/
	ST_TOKUISAKI_SHUBETSU			nchar           (1)			not null,	/* 	得意先種別						*/
	NO_OYASHOTEN					nchar           (6)			not null,	/* 	親商店コード					*/
	NO_SOSHIKI_1					nvarchar        (6)			not null,	/* 	組織管理番号１					*/
	NO_SOSHIKI_2					nvarchar        (6)			not null,	/* 	組織管理番号２					*/
	CD_KEIYAKU_GROUP				nchar           (6)			not null,	/* 	契約グループコード				*/
	NO_KEIYAKU_SEDAI				nchar           (2)			not null,	/* 	契約世代番号					*/
	CD_SHODAN_GROUP					nchar           (6)			not null,	/* 	商談グループコード				*/
	NO_SHODAN_SEDAI					nchar           (2)			not null,	/* 	商談世代番号					*/
	CD_JUCHU_GROUP					nchar           (6)			not null,	/* 	受注グループコード				*/
	NO_JUCHU_SEDAI					nchar           (2)			not null,	/* 	受注世代番号					*/
	CD_NOHIN_GROUP					nchar           (6)			not null,	/* 	納品グループコード				*/
	NO_NOHIN_SEDAI					nchar           (2)			not null,	/* 	納品世代番号					*/
	CD_SEIKYU_GROUP					nchar           (6)			not null,	/* 	請求グループコード				*/
	NO_SEIKYU_SEDAI					nchar           (2)			not null,	/* 	請求世代番号					*/
	CD_NYUKIN_GROUP					nchar           (6)			not null,	/* 	入金グループコード				*/
	NO_NYUKIN_SEDAI					nchar           (2)			not null,	/* 	入金世代番号					*/
	ST_KAZEI						nchar           (1)			not null,	/* 	課税区分						*/
	ST_SHOHIZEI_KEISAN				nchar           (1)			not null,	/* 	消費税計算区分					*/
	ST_NAIBU_URIAGE_KOZA			nchar           (1)			not null,	/* 	内部売上口座区分				*/
	ST_KEIHI_URIAGE_KOZA			nchar           (1)			not null,	/* 	経費売上口座区分				*/
	ST_TORIHIKISAKI_KOZA			nchar           (1)			not null,	/* 	取引先口座区分					*/
	ST_NAIBU_URIAGE					nchar           (1)			not null,	/* 	内部売上区分					*/
	CD_EOS_TOKUISAKI_HIMOZUKE		nchar           (20)		not null,	/* 	EOS紐づけコード					*/
	CD_AITE_TOKUISAKI				nchar           (10)		not null,	/* 	相手得意先コード				*/
	NM_AITE_KAISHA					nvarchar        (50)		not null,	/* 	相手会社名						*/
	NM_AITEKAISHA_YOMI				nvarchar        (30)		not null,	/* 	相手会社名読み					*/
	CD_AITE_KAISHA					nchar           (10)		not null,	/* 	相手会社コード					*/
	NM_AITE_SHOTEN					nvarchar        (40)		not null,	/* 	相手商店名						*/
	NM_AITE_SHOTEN_YOMI				nvarchar        (30)		not null,	/* 	相手商店名読み					*/
	ST_TESURYO						nchar           (1)			not null,	/* 	手数料区分						*/
	CD_BUMON_KAISHA					nchar           (6)			not null,	/* 	部門会社コード					*/
	ST_CHOKKATSU_FC					nchar           (1)			not null,	/* 	直轄FC区分						*/
	NM_TOKUISAKI_RYAKUSHO			nvarchar        (10)		not null,	/* 	得意先略称						*/
	FG_HATCHUYOTEISU_SAKUSEI_UMU	nchar           (1)			not null,	/* 	発注予定数作成有無フラグ		*/
	FG_GENTEN_KAHI					nchar           (1)			not null,	/* 	現添可否フラグ					*/
	ST_ORDERHYO_UMU					nchar           (1)			not null,	/* 	オーダー表有無区分				*/
	CD_ORDER_HYO_GYOMU_KYOTEN		nchar           (3)			not null,	/* 	オーダー表出力業務拠点			*/
	ST_GYOMU_SYSTEM					nchar           (1)			not null,	/* 	業務システム区分				*/
	ST_MISE_SHUBETSU				nchar           (1)			not null,	/* 	店種別							*/
	DT_OYA_KAISHI					nchar           (8)			not null,	/* 	親開始日						*/
	NM_JUSHO_YOMI2					nvarchar        (30)		not null,	/* 	住所読み２						*/
	NM_KAISHA						nvarchar        (40)		not null,	/* 	会社名							*/
	NM_KAISHA_YOMI					nvarchar        (30)		not null,	/* 	会社名読み						*/
	NM_RYAKUSHO_KAISHA				nvarchar        (16)		not null,	/* 	会社名略称						*/
	NM_RYAKUSHO_KAISHA_YOMI			nvarchar        (15)		not null,	/* 	会社名略称読み					*/
	NM_TOKUISAKI_RYAKUSHO_YOMI		nvarchar        (10)		not null,	/* 	得意先略称読み					*/
	ST_SAND_HATCHU					nchar           (1)			not null,	/* 	サンド発注区分					*/
	ST_UEHARI_LABEL_HYOJI			nchar           (2)			not null,	/* 	上貼りラベル表示区分			*/
	ST_GYOMU_SHORI_PATTERN			nchar           (3)			not null,	/* 	業務処理パターン				*/
	CD_HAISHO_BIN					nchar           (3)			not null,	/* 	配送便コード					*/
	NU_ENKYORI_LEADTIME				numeric         (3)			not null,	/* 	遠距離リードタイム				*/
	IF_IDO							nvarchar        (3)			not null,	/* 	緯度							*/
	IF_KEIDO						nvarchar        (3)			not null,	/* 	経度							*/
	NU_KEMPIN_JIKAN					numeric         (2)			not null,	/* 	検品時間						*/
	TM_NOHIN_KANO_JI				nvarchar        (4)			not null,	/* 	納品可能時刻（自）				*/
	TM_NOHIN_KANO_ITARU				nvarchar        (4)			not null,	/* 	納品可能時刻（至）				*/
	CD_HIGASHINIHON_TEMBAN			nchar           (5)			not null,	/* 	東日本店番						*/
	ST_URIKAKE_KANRI				nchar           (1)			not null,	/* 	売掛管理区分					*/
	IF_SHUTTEN_KEIYAKU_SAKI			nvarchar        (80)		not null,	/* 	出店契約先						*/
	IF_FC_KEIYAKU_SAKI				nvarchar        (80)		not null,	/* 	FC契約先						*/
	IF_BT1_NUMBER					nchar           (12)		not null,	/* 	BT1ナンバー						*/
	IF_BG3_NUMBER					nchar           (12)		not null,	/* 	BG3ナンバー						*/
	CD_TOKUISAKI_GROUP_9			nchar           (7)			not null,	/* 	得意先グループ９				*/
	MN_YOSHIN_GENDO_GAKU			numeric         (13)		not null,	/* 	与信限度額						*/
	DT_ZENKAI_HENKO					nchar           (8)			not null,	/* 	前回変更日						*/
	ST_OKURIJO_HAKKO				nchar           (2)			not null,	/* 	送り状発行区分					*/
	ST_YUSEN_JUN					nchar           (3)			not null,	/* 	便優先順区分					*/
	NU_HAITA						numeric         (5)			not null,	/* 	排他カウンタ					*/
	DT_RENKEI						nchar           (14)		not null,	/* 	連携日時						*/
	CD_RITCHI						nchar           (2)			not null,	/* 	立地区分						*/
	CD_TANTOSHA						nchar           (7)			not null,	/* 	担当者コード					*/
	DT_KEI_HO_NYUKIN				nchar           (8)			not null,	/* 	契約時 営業保証金 入金日		*/
	MN_KEI_HO_HOSHO_KINGAKU			numeric         (13)		not null,	/* 	契約時 営業保証金 保証金額		*/
	NM_KEI_HO_JISHATANTO			nvarchar        (20)		not null,	/* 	契約時 営業保証金 自社担当		*/
	NM_KEI_HO_AITETANTO				nvarchar        (20)		not null,	/* 	契約時 営業保証金 相手担当		*/
	DT_KAI_HO_NYUKIN				nchar           (8)			not null,	/* 	解約時 営業保証金 入金日		*/
	MN_KAI_HO_HOSHO_KINGAKU			numeric         (13)		not null,	/* 	解約時 営業保証金 保証金額		*/
	NM_KAI_HO_JISHATANTO			nvarchar        (20)		not null,	/* 	解約時 営業保証金 自社担当		*/
	NM_KAI_HO_AITETANTO				nvarchar        (20)		not null,	/* 	解約時 営業保証金 相手担当		*/
	NM_SHUKKA_LABEL_TENMEI			nvarchar        (40)		not null,	/* 	出荷ラベル店名					*/
	
	createdAt datetime not null,		/*	データ挿入日				*/
	updatedAt datetime not null,		/*	データ更新日				*/

	constraint pk_TB_MTOKUISAKI primary key (CD_TOKUISAKI,DT_TEKIYOKAISHI)
);
