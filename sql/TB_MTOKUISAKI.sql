/*
 *
 *	���Ӑ�}�X�^
 *
 *	$Id: TB_MTOKUISAKI.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_MTOKUISAKI (
	
	CD_TOKUISAKI					char			(6)			not null,	/* 	���Ӑ�R�[�h					*/
	DT_TEKIYOKAISHI					char            (8)			not null,	/* 	�K�p�J�n��						*/
	DT_TEKIYOMUKO					char            (8)			not null,	/* 	�K�p��������					*/
	DT_TOROKU_NICHIJI				char            (14)		not null,	/* 	�o�^����						*/
	DT_KOSHIN_NICHIJI				char            (14)		not null,	/* 	�X�V����						*/
	CD_HENKOSHA						char            (10)		not null,	/* 	�ύX��							*/
	NM_TOKUISAKI					nvarchar        (40)		not null,	/* 	���Ӑ於						*/
	NM_TOKUISAKI_YOMI				nvarchar        (18)		not null,	/* 	���Ӑ於�ǂ�					*/
	NM_JUSHO_1						nvarchar        (60)		not null,	/* 	�Z���P							*/
	NM_JUSHO_2						nvarchar        (30)		not null,	/* 	�Z���Q							*/
	NM_JUSHO_YOMI					nvarchar        (60)		not null,	/* 	�Z���ǂ�						*/
	IF_YUBIN_BANGO					nvarchar        (8)			not null,	/* 	�X�֔ԍ�						*/
	IF_DENWA_BANGO					nvarchar        (17)		not null,	/* 	�d�b�ԍ�						*/
	IF_KENSAKUYO_DENWA_BANGO		nvarchar        (11)		not null,	/* 	�����p�d�b�ԍ�					*/
	IF_FAX_BANGO					nvarchar        (17)		not null,	/* 	FAX�ԍ�							*/
	CD_CHIIKI						char            (5)			not null,	/* 	�n��R�[�h						*/
	DT_KAITEN						char            (8)			not null,	/* 	�J�X��							*/
	DT_HEITEN						char            (8)			not null,	/* 	�X��							*/
	DT_KEIYAKU						char            (8)			not null,	/* 	�_���							*/
	DT_KAIYAKU						char            (8)			not null,	/* 	����							*/
	ST_CHANEL						char            (2)			not null,	/* 	�`���l���敪					*/
	CD_KANKATSU_EIGYOBUMON			char            (6)			not null,	/* 	�Ǌ��c�ƕ���R�[�h				*/
	CD_TANTO_GYOMUKYOTEN			char            (3)			not null,	/* 	�S���Ɩ����_					*/
	ST_GYOTAI						char            (1)			not null,	/* 	�Ƒԋ敪						*/
	ST_TOKUISAKI_SHIKIBETSU			char            (1)			not null,	/* 	���Ӑ掯�ʋ敪					*/
	ST_TOKUISAKI_SHUBETSU			char            (1)			not null,	/* 	���Ӑ���						*/
	NO_OYASHOTEN					char            (6)			not null,	/* 	�e���X�R�[�h					*/
	NO_SOSHIKI_1					nvarchar        (6)			not null,	/* 	�g�D�Ǘ��ԍ��P					*/
	NO_SOSHIKI_2					nvarchar        (6)			not null,	/* 	�g�D�Ǘ��ԍ��Q					*/
	CD_KEIYAKU_GROUP				char            (6)			not null,	/* 	�_��O���[�v�R�[�h				*/
	NO_KEIYAKU_SEDAI				char            (2)			not null,	/* 	�_�񐢑�ԍ�					*/
	CD_SHODAN_GROUP					char            (6)			not null,	/* 	���k�O���[�v�R�[�h				*/
	NO_SHODAN_SEDAI					char            (2)			not null,	/* 	���k����ԍ�					*/
	CD_JUCHU_GROUP					char            (6)			not null,	/* 	�󒍃O���[�v�R�[�h				*/
	NO_JUCHU_SEDAI					char            (2)			not null,	/* 	�󒍐���ԍ�					*/
	CD_NOHIN_GROUP					char            (6)			not null,	/* 	�[�i�O���[�v�R�[�h				*/
	NO_NOHIN_SEDAI					char            (2)			not null,	/* 	�[�i����ԍ�					*/
	CD_SEIKYU_GROUP					char            (6)			not null,	/* 	�����O���[�v�R�[�h				*/
	NO_SEIKYU_SEDAI					char            (2)			not null,	/* 	��������ԍ�					*/
	CD_NYUKIN_GROUP					char            (6)			not null,	/* 	�����O���[�v�R�[�h				*/
	NO_NYUKIN_SEDAI					char            (2)			not null,	/* 	��������ԍ�					*/
	ST_KAZEI						char            (1)			not null,	/* 	�ېŋ敪						*/
	ST_SHOHIZEI_KEISAN				char            (1)			not null,	/* 	����Ōv�Z�敪					*/
	ST_NAIBU_URIAGE_KOZA			char            (1)			not null,	/* 	������������敪				*/
	ST_KEIHI_URIAGE_KOZA			char            (1)			not null,	/* 	�o�������敪				*/
	ST_TORIHIKISAKI_KOZA			char            (1)			not null,	/* 	���������敪					*/
	ST_NAIBU_URIAGE					char            (1)			not null,	/* 	��������敪					*/
	CD_EOS_TOKUISAKI_HIMOZUKE		char            (20)		not null,	/* 	EOS�R�Â��R�[�h					*/
	CD_AITE_TOKUISAKI				char            (10)		not null,	/* 	���蓾�Ӑ�R�[�h				*/
	NM_AITE_KAISHA					nvarchar        (50)		not null,	/* 	�����Ж�						*/
	NM_AITEKAISHA_YOMI				nvarchar        (30)		not null,	/* 	�����Ж��ǂ�					*/
	CD_AITE_KAISHA					char            (10)		not null,	/* 	�����ЃR�[�h					*/
	NM_AITE_SHOTEN					nvarchar        (40)		not null,	/* 	���菤�X��						*/
	NM_AITE_SHOTEN_YOMI				nvarchar        (30)		not null,	/* 	���菤�X���ǂ�					*/
	ST_TESURYO						char            (1)			not null,	/* 	�萔���敪						*/
	CD_BUMON_KAISHA					char            (6)			not null,	/* 	�����ЃR�[�h					*/
	ST_CHOKKATSU_FC					char            (1)			not null,	/* 	����FC�敪						*/
	NM_TOKUISAKI_RYAKUSHO			nvarchar        (10)		not null,	/* 	���Ӑ旪��						*/
	FG_HATCHUYOTEISU_SAKUSEI_UMU	char            (1)			not null,	/* 	�����\�萔�쐬�L���t���O		*/
	FG_GENTEN_KAHI					char            (1)			not null,	/* 	���Y�ۃt���O					*/
	ST_ORDERHYO_UMU					char            (1)			not null,	/* 	�I�[�_�[�\�L���敪				*/
	CD_ORDER_HYO_GYOMU_KYOTEN		char            (3)			not null,	/* 	�I�[�_�[�\�o�͋Ɩ����_			*/
	ST_GYOMU_SYSTEM					char            (1)			not null,	/* 	�Ɩ��V�X�e���敪				*/
	ST_MISE_SHUBETSU				char            (1)			not null,	/* 	�X���							*/
	DT_OYA_KAISHI					char            (8)			not null,	/* 	�e�J�n��						*/
	NM_JUSHO_YOMI2					nvarchar        (30)		not null,	/* 	�Z���ǂ݂Q						*/
	NM_KAISHA						nvarchar        (40)		not null,	/* 	��Ж�							*/
	NM_KAISHA_YOMI					nvarchar        (30)		not null,	/* 	��Ж��ǂ�						*/
	NM_RYAKUSHO_KAISHA				nvarchar        (16)		not null,	/* 	��Ж�����						*/
	NM_RYAKUSHO_KAISHA_YOMI			nvarchar        (15)		not null,	/* 	��Ж����̓ǂ�					*/
	NM_TOKUISAKI_RYAKUSHO_YOMI		nvarchar        (10)		not null,	/* 	���Ӑ旪�̓ǂ�					*/
	ST_SAND_HATCHU					char            (1)			not null,	/* 	�T���h�����敪					*/
	ST_UEHARI_LABEL_HYOJI			char            (2)			not null,	/* 	��\�胉�x���\���敪			*/
	ST_GYOMU_SHORI_PATTERN			char            (3)			not null,	/* 	�Ɩ������p�^�[��				*/
	CD_HAISHO_BIN					char            (3)			not null,	/* 	�z���փR�[�h					*/
	NU_ENKYORI_LEADTIME				numeric         (3)			not null,	/* 	���������[�h�^�C��				*/
	IF_IDO							nvarchar        (3)			not null,	/* 	�ܓx							*/
	IF_KEIDO						nvarchar        (3)			not null,	/* 	�o�x							*/
	NU_KEMPIN_JIKAN					numeric         (2)			not null,	/* 	���i����						*/
	TM_NOHIN_KANO_JI				nvarchar        (4)			not null,	/* 	�[�i�\�����i���j				*/
	TM_NOHIN_KANO_ITARU				nvarchar        (4)			not null,	/* 	�[�i�\�����i���j				*/
	CD_HIGASHINIHON_TEMBAN			char            (5)			not null,	/* 	�����{�X��						*/
	ST_URIKAKE_KANRI				char            (1)			not null,	/* 	���|�Ǘ��敪					*/
	IF_SHUTTEN_KEIYAKU_SAKI			nvarchar        (80)		not null,	/* 	�o�X�_���						*/
	IF_FC_KEIYAKU_SAKI				nvarchar        (80)		not null,	/* 	FC�_���						*/
	IF_BT1_NUMBER					char            (12)		not null,	/* 	BT1�i���o�[						*/
	IF_BG3_NUMBER					char            (12)		not null,	/* 	BG3�i���o�[						*/
	CD_TOKUISAKI_GROUP_9			char            (7)			not null,	/* 	���Ӑ�O���[�v�X				*/
	MN_YOSHIN_GENDO_GAKU			numeric         (13)		not null,	/* 	�^�M���x�z						*/
	DT_ZENKAI_HENKO					char            (8)			not null,	/* 	�O��ύX��						*/
	ST_OKURIJO_HAKKO				char            (2)			not null,	/* 	����󔭍s�敪					*/
	ST_YUSEN_JUN					char            (3)			not null,	/* 	�֗D�揇�敪					*/
	NU_HAITA						numeric         (5)			not null,	/* 	�r���J�E���^					*/
	DT_RENKEI						char            (14)		not null,	/* 	�A�g����						*/
	CD_RITCHI						char            (2)			not null,	/* 	���n�敪						*/
	CD_TANTOSHA						char            (7)			not null,	/* 	�S���҃R�[�h					*/
	DT_KEI_HO_NYUKIN				char            (8)			not null,	/* 	�_�� �c�ƕۏ؋� ������		*/
	MN_KEI_HO_HOSHO_KINGAKU			numeric         (13)		not null,	/* 	�_�� �c�ƕۏ؋� �ۏ؋��z		*/
	NM_KEI_HO_JISHATANTO			nvarchar        (20)		not null,	/* 	�_�� �c�ƕۏ؋� ���ВS��		*/
	NM_KEI_HO_AITETANTO				nvarchar        (20)		not null,	/* 	�_�� �c�ƕۏ؋� ����S��		*/
	DT_KAI_HO_NYUKIN				char            (8)			not null,	/* 	��� �c�ƕۏ؋� ������		*/
	MN_KAI_HO_HOSHO_KINGAKU			numeric         (13)		not null,	/* 	��� �c�ƕۏ؋� �ۏ؋��z		*/
	NM_KAI_HO_JISHATANTO			nvarchar        (20)		not null,	/* 	��� �c�ƕۏ؋� ���ВS��		*/
	NM_KAI_HO_AITETANTO				nvarchar        (20)		not null,	/* 	��� �c�ƕۏ؋� ����S��		*/
	NM_SHUKKA_LABEL_TENMEI			nvarchar        (40)		not null,	/* 	�o�׃��x���X��					*/
	
	createdAt datetime not null,		/*	�f�[�^�}����				*/
	updatedAt datetime not null,		/*	�f�[�^�X�V��				*/

	constraint pk_TB_MTOKUISAKI primary key (CD_TOKUISAKI,DT_TEKIYOKAISHI)
);
