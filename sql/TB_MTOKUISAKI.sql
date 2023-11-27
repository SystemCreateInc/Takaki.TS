/*
 *
 *	���Ӑ�}�X�^
 *
 *	$Id: TB_MTOKUISAKI.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_MTOKUISAKI (
	
	CD_TOKUISAKI					nchar			(6)			not null,	/* 	���Ӑ�R�[�h					*/
	DT_TEKIYOKAISHI					nchar           (8)			not null,	/* 	�K�p�J�n��						*/
	DT_TEKIYOMUKO					nchar           (8)			not null,	/* 	�K�p��������					*/
	DT_TOROKU_NICHIJI				nchar           (14)		not null,	/* 	�o�^����						*/
	DT_KOSHIN_NICHIJI				nchar           (14)		not null,	/* 	�X�V����						*/
	CD_HENKOSHA						nchar           (10)		not null,	/* 	�ύX��							*/
	NM_TOKUISAKI					nvarchar        (40)		not null,	/* 	���Ӑ於						*/
	NM_TOKUISAKI_YOMI				nvarchar        (18)		not null,	/* 	���Ӑ於�ǂ�					*/
	NM_JUSHO_1						nvarchar        (60)		not null,	/* 	�Z���P							*/
	NM_JUSHO_2						nvarchar        (30)		not null,	/* 	�Z���Q							*/
	NM_JUSHO_YOMI					nvarchar        (60)		not null,	/* 	�Z���ǂ�						*/
	IF_YUBIN_BANGO					nvarchar        (8)			not null,	/* 	�X�֔ԍ�						*/
	IF_DENWA_BANGO					nvarchar        (17)		not null,	/* 	�d�b�ԍ�						*/
	IF_KENSAKUYO_DENWA_BANGO		nvarchar        (11)		not null,	/* 	�����p�d�b�ԍ�					*/
	IF_FAX_BANGO					nvarchar        (17)		not null,	/* 	FAX�ԍ�							*/
	CD_CHIIKI						nchar           (5)			not null,	/* 	�n��R�[�h						*/
	DT_KAITEN						nchar           (8)			not null,	/* 	�J�X��							*/
	DT_HEITEN						nchar           (8)			not null,	/* 	�X��							*/
	DT_KEIYAKU						nchar           (8)			not null,	/* 	�_���							*/
	DT_KAIYAKU						nchar           (8)			not null,	/* 	����							*/
	ST_CHANEL						nchar           (2)			not null,	/* 	�`���l���敪					*/
	CD_KANKATSU_EIGYOBUMON			nchar           (6)			not null,	/* 	�Ǌ��c�ƕ���R�[�h				*/
	CD_TANTO_GYOMUKYOTEN			nchar           (3)			not null,	/* 	�S���Ɩ����_					*/
	ST_GYOTAI						nchar           (1)			not null,	/* 	�Ƒԋ敪						*/
	ST_TOKUISAKI_SHIKIBETSU			nchar           (1)			not null,	/* 	���Ӑ掯�ʋ敪					*/
	ST_TOKUISAKI_SHUBETSU			nchar           (1)			not null,	/* 	���Ӑ���						*/
	NO_OYASHOTEN					nchar           (6)			not null,	/* 	�e���X�R�[�h					*/
	NO_SOSHIKI_1					nvarchar        (6)			not null,	/* 	�g�D�Ǘ��ԍ��P					*/
	NO_SOSHIKI_2					nvarchar        (6)			not null,	/* 	�g�D�Ǘ��ԍ��Q					*/
	CD_KEIYAKU_GROUP				nchar           (6)			not null,	/* 	�_��O���[�v�R�[�h				*/
	NO_KEIYAKU_SEDAI				nchar           (2)			not null,	/* 	�_�񐢑�ԍ�					*/
	CD_SHODAN_GROUP					nchar           (6)			not null,	/* 	���k�O���[�v�R�[�h				*/
	NO_SHODAN_SEDAI					nchar           (2)			not null,	/* 	���k����ԍ�					*/
	CD_JUCHU_GROUP					nchar           (6)			not null,	/* 	�󒍃O���[�v�R�[�h				*/
	NO_JUCHU_SEDAI					nchar           (2)			not null,	/* 	�󒍐���ԍ�					*/
	CD_NOHIN_GROUP					nchar           (6)			not null,	/* 	�[�i�O���[�v�R�[�h				*/
	NO_NOHIN_SEDAI					nchar           (2)			not null,	/* 	�[�i����ԍ�					*/
	CD_SEIKYU_GROUP					nchar           (6)			not null,	/* 	�����O���[�v�R�[�h				*/
	NO_SEIKYU_SEDAI					nchar           (2)			not null,	/* 	��������ԍ�					*/
	CD_NYUKIN_GROUP					nchar           (6)			not null,	/* 	�����O���[�v�R�[�h				*/
	NO_NYUKIN_SEDAI					nchar           (2)			not null,	/* 	��������ԍ�					*/
	ST_KAZEI						nchar           (1)			not null,	/* 	�ېŋ敪						*/
	ST_SHOHIZEI_KEISAN				nchar           (1)			not null,	/* 	����Ōv�Z�敪					*/
	ST_NAIBU_URIAGE_KOZA			nchar           (1)			not null,	/* 	������������敪				*/
	ST_KEIHI_URIAGE_KOZA			nchar           (1)			not null,	/* 	�o�������敪				*/
	ST_TORIHIKISAKI_KOZA			nchar           (1)			not null,	/* 	���������敪					*/
	ST_NAIBU_URIAGE					nchar           (1)			not null,	/* 	��������敪					*/
	CD_EOS_TOKUISAKI_HIMOZUKE		nchar           (20)		not null,	/* 	EOS�R�Â��R�[�h					*/
	CD_AITE_TOKUISAKI				nchar           (10)		not null,	/* 	���蓾�Ӑ�R�[�h				*/
	NM_AITE_KAISHA					nvarchar        (50)		not null,	/* 	�����Ж�						*/
	NM_AITEKAISHA_YOMI				nvarchar        (30)		not null,	/* 	�����Ж��ǂ�					*/
	CD_AITE_KAISHA					nchar           (10)		not null,	/* 	�����ЃR�[�h					*/
	NM_AITE_SHOTEN					nvarchar        (40)		not null,	/* 	���菤�X��						*/
	NM_AITE_SHOTEN_YOMI				nvarchar        (30)		not null,	/* 	���菤�X���ǂ�					*/
	ST_TESURYO						nchar           (1)			not null,	/* 	�萔���敪						*/
	CD_BUMON_KAISHA					nchar           (6)			not null,	/* 	�����ЃR�[�h					*/
	ST_CHOKKATSU_FC					nchar           (1)			not null,	/* 	����FC�敪						*/
	NM_TOKUISAKI_RYAKUSHO			nvarchar        (10)		not null,	/* 	���Ӑ旪��						*/
	FG_HATCHUYOTEISU_SAKUSEI_UMU	nchar           (1)			not null,	/* 	�����\�萔�쐬�L���t���O		*/
	FG_GENTEN_KAHI					nchar           (1)			not null,	/* 	���Y�ۃt���O					*/
	ST_ORDERHYO_UMU					nchar           (1)			not null,	/* 	�I�[�_�[�\�L���敪				*/
	CD_ORDER_HYO_GYOMU_KYOTEN		nchar           (3)			not null,	/* 	�I�[�_�[�\�o�͋Ɩ����_			*/
	ST_GYOMU_SYSTEM					nchar           (1)			not null,	/* 	�Ɩ��V�X�e���敪				*/
	ST_MISE_SHUBETSU				nchar           (1)			not null,	/* 	�X���							*/
	DT_OYA_KAISHI					nchar           (8)			not null,	/* 	�e�J�n��						*/
	NM_JUSHO_YOMI2					nvarchar        (30)		not null,	/* 	�Z���ǂ݂Q						*/
	NM_KAISHA						nvarchar        (40)		not null,	/* 	��Ж�							*/
	NM_KAISHA_YOMI					nvarchar        (30)		not null,	/* 	��Ж��ǂ�						*/
	NM_RYAKUSHO_KAISHA				nvarchar        (16)		not null,	/* 	��Ж�����						*/
	NM_RYAKUSHO_KAISHA_YOMI			nvarchar        (15)		not null,	/* 	��Ж����̓ǂ�					*/
	NM_TOKUISAKI_RYAKUSHO_YOMI		nvarchar        (10)		not null,	/* 	���Ӑ旪�̓ǂ�					*/
	ST_SAND_HATCHU					nchar           (1)			not null,	/* 	�T���h�����敪					*/
	ST_UEHARI_LABEL_HYOJI			nchar           (2)			not null,	/* 	��\�胉�x���\���敪			*/
	ST_GYOMU_SHORI_PATTERN			nchar           (3)			not null,	/* 	�Ɩ������p�^�[��				*/
	CD_HAISHO_BIN					nchar           (3)			not null,	/* 	�z���փR�[�h					*/
	NU_ENKYORI_LEADTIME				numeric         (3)			not null,	/* 	���������[�h�^�C��				*/
	IF_IDO							nvarchar        (3)			not null,	/* 	�ܓx							*/
	IF_KEIDO						nvarchar        (3)			not null,	/* 	�o�x							*/
	NU_KEMPIN_JIKAN					numeric         (2)			not null,	/* 	���i����						*/
	TM_NOHIN_KANO_JI				nvarchar        (4)			not null,	/* 	�[�i�\�����i���j				*/
	TM_NOHIN_KANO_ITARU				nvarchar        (4)			not null,	/* 	�[�i�\�����i���j				*/
	CD_HIGASHINIHON_TEMBAN			nchar           (5)			not null,	/* 	�����{�X��						*/
	ST_URIKAKE_KANRI				nchar           (1)			not null,	/* 	���|�Ǘ��敪					*/
	IF_SHUTTEN_KEIYAKU_SAKI			nvarchar        (80)		not null,	/* 	�o�X�_���						*/
	IF_FC_KEIYAKU_SAKI				nvarchar        (80)		not null,	/* 	FC�_���						*/
	IF_BT1_NUMBER					nchar           (12)		not null,	/* 	BT1�i���o�[						*/
	IF_BG3_NUMBER					nchar           (12)		not null,	/* 	BG3�i���o�[						*/
	CD_TOKUISAKI_GROUP_9			nchar           (7)			not null,	/* 	���Ӑ�O���[�v�X				*/
	MN_YOSHIN_GENDO_GAKU			numeric         (13)		not null,	/* 	�^�M���x�z						*/
	DT_ZENKAI_HENKO					nchar           (8)			not null,	/* 	�O��ύX��						*/
	ST_OKURIJO_HAKKO				nchar           (2)			not null,	/* 	����󔭍s�敪					*/
	ST_YUSEN_JUN					nchar           (3)			not null,	/* 	�֗D�揇�敪					*/
	NU_HAITA						numeric         (5)			not null,	/* 	�r���J�E���^					*/
	DT_RENKEI						nchar           (14)		not null,	/* 	�A�g����						*/
	CD_RITCHI						nchar           (2)			not null,	/* 	���n�敪						*/
	CD_TANTOSHA						nchar           (7)			not null,	/* 	�S���҃R�[�h					*/
	DT_KEI_HO_NYUKIN				nchar           (8)			not null,	/* 	�_�� �c�ƕۏ؋� ������		*/
	MN_KEI_HO_HOSHO_KINGAKU			numeric         (13)		not null,	/* 	�_�� �c�ƕۏ؋� �ۏ؋��z		*/
	NM_KEI_HO_JISHATANTO			nvarchar        (20)		not null,	/* 	�_�� �c�ƕۏ؋� ���ВS��		*/
	NM_KEI_HO_AITETANTO				nvarchar        (20)		not null,	/* 	�_�� �c�ƕۏ؋� ����S��		*/
	DT_KAI_HO_NYUKIN				nchar           (8)			not null,	/* 	��� �c�ƕۏ؋� ������		*/
	MN_KAI_HO_HOSHO_KINGAKU			numeric         (13)		not null,	/* 	��� �c�ƕۏ؋� �ۏ؋��z		*/
	NM_KAI_HO_JISHATANTO			nvarchar        (20)		not null,	/* 	��� �c�ƕۏ؋� ���ВS��		*/
	NM_KAI_HO_AITETANTO				nvarchar        (20)		not null,	/* 	��� �c�ƕۏ؋� ����S��		*/
	NM_SHUKKA_LABEL_TENMEI			nvarchar        (40)		not null,	/* 	�o�׃��x���X��					*/
	
	createdAt datetime not null,		/*	�f�[�^�}����				*/
	updatedAt datetime not null,		/*	�f�[�^�X�V��				*/

	constraint pk_TB_MTOKUISAKI primary key (CD_TOKUISAKI,DT_TEKIYOKAISHI)
);
