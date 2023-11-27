/*
 *
 *	�o�׃o�b�`�}�X�^
 *
 *	$Id: TB_MSHUKKA_BATCH.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_MSHUKKA_BATCH (
	
	CD_SHUKKA_BATCH					nchar			(5)			not null,	/* 	�o�׃o�b�`�R�[�h				*/
	DT_TEKIYOKAISHI					nchar			(8)			not null,	/* 	�K�p�J�n��						*/
	DT_TEKIYOMUKO					nchar			(8)			not null,	/* 	�K�p������						*/
	DT_TOROKU_NICHIJI				nchar			(14)		not null,	/* 	�o�^����						*/
	DT_KOSHIN_NICHIJI				nchar			(14)		not null,	/* 	�X�V����						*/
	CD_HENKOSHA						nchar			(10)		not null,	/* 	�ύX�҃R�[�h					*/
	NM_SHUKKA_BATCH					nvarchar		(40)		not null,	/* 	�o�׃o�b�`����					*/
	CD_SHIWAKE_KYOTEN				nchar			(4)			not null,	/* 	�d�����_�R�[�h					*/
	TM_SHIWAKE_KAISHI				nchar			(4)			not null,	/* 	�d���J�n����					*/
	TM_SHIWAKE_SHURYO				nchar			(4)			not null,	/* 	�d���I������					*/
	CD_SHUKKA_BATCH_GROUP			nchar			(6)			not null,	/* 	�o�׃o�b�`�O���[�v				*/
	ST_OSHIWAKEHYO_SHUBETSU			nchar			(1)			not null,	/* 	��d���\��ʋ敪				*/
	ST_CHUSHIWAKEHYO_SHUBETSU		nchar			(1)			not null,	/* 	���d���\��ʋ敪				*/
	ST_KOSHIWAKEHYO_SHUBETSU		nchar			(1)			not null,	/* 	���d���\��ʋ敪				*/
	FG_TOKUISAKI_SHUKEI_1			nchar			(1)			not null,	/* 	���Ӑ�ʏW�v�o�̓t���O�P		*/
	FG_TOKUISAKI_SHUKEI_2			nchar			(1)			not null,	/* 	���Ӑ�ʏW�v�o�̓t���O�Q		*/
	FG_TOKUISAKI_SHUKEI_3			nchar			(1)			not null,	/* 	���Ӑ�ʏW�v�o�̓t���O�R		*/
	FG_TOKUISAKI_SHUKEI_4			nchar			(1)			not null,	/* 	���Ӑ�ʏW�v�o�̓t���O�S		*/
	FG_TOKUISAKI_SHUKEI_5			nchar           (1)			not null,	/* 	���Ӑ�ʏW�v�o�̓t���O�T		*/
	FG_TOKUISAKI_SHUKEI_6			nchar           (1)			not null,	/* 	���Ӑ�ʏW�v�o�̓t���O�U		*/
	FG_HAISOUBIN_BETSU_SHUKEI_UMU	nchar           (1)			not null,	/* 	�z���֕ʎ󒍏W�v�o�͗L��		*/
	FG_COURSE_BETSU_SHUKEI_UMU		nchar           (1)			not null,	/* 	�R�[�X�ʎ󒍏W�v�o�͗L��		*/
	FG_JUCHUJOKYOHYO				nchar           (1)			not null,	/* 	�󒍏󋵕\�o�̓t���O			*/
	FG_SAND_HIKITORI_HYO			nchar           (1)			not null,	/* 	�T���h����\�o�̓t���O			*/
	ST_DPS_SHUBETSU					nchar           (2)			not null,	/* 	�c�o�r���						*/
	NU_SHIWAKE_LEAD_TIME			numeric         (3)			not null,	/* 	�d�����[�h�^�C��				*/
	ST_LEAD_TIME_SEIGYO				nchar           (1)			not null,	/* 	���[�h�^�C������敪			*/
	FG_YOBI_1						nchar           (1)			not null,	/* 	�\���t���O�P					*/
	FG_YOBI_2						nchar           (1)			not null,	/* 	�\���t���O�Q					*/
	FG_YOBI_3						nchar           (1)			not null,	/* 	�\���t���O�R					*/
	FG_YOBI_4						nchar           (1)			not null,	/* 	�\���t���O�S					*/
	FG_YOBI_5						nchar           (1)			not null,	/* 	�\���t���O�T					*/
	ST_YOBI_1						nchar           (1)			not null,	/* 	�\���敪�P						*/
	ST_YOBI_2						nchar           (1)			not null,	/* 	�\���敪�Q						*/
	ST_YOBI_3						nchar           (1)			not null,	/* 	�\���敪�R						*/
	ST_YOBI_4						nchar           (1)			not null,	/* 	�\���敪�S						*/
	ST_YOBI_5						nchar           (1)			not null,	/* 	�\���敪�T						*/
	NU_HAITA						numeric         (5)			not null,	/* 	�r���J�E���^					*/
	DT_RENKEI						nchar           (14)		not null,	/* 	�A�g����						*/
	
	createdAt datetime not null,		/*	�f�[�^�}����				*/
	updatedAt datetime not null,		/*	�f�[�^�X�V��				*/

	constraint pk_TB_MSHUKKA_BATCH primary key (CD_SHUKKA_BATCH	,DT_TEKIYOKAISHI)
);

