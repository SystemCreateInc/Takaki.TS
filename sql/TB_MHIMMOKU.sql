/*
 *
 *	iÚ}X^
 *
 *	$Id: TB_MHIMMOKU.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

drop table TB_MHIMMOKU;
create table TB_MHIMMOKU (
	CD_HIMBAN						            nchar           (9)         not null,	/* 	iÔ								*/
	DT_TEKIYOKAISHI                 nchar           (8)			    not null,   /* 	KpJnú                          */
	DT_TEKIYOMUKO                   nchar           (8)         not null,  /* 	Kp³øú                          */
	DT_TOROKU_NICHIJI               nchar           (14)        not null,  /* 	o^ú                            */
	DT_KOSHIN_NICHIJI               nchar           (14)        not null,  /* 	XVú                            */
	CD_HENKOSHA                     nchar           (10)        not null,  /* 	ÏXÒR[h                        */
	NM_HIN_SEISHIKIMEI              nvarchar        (60)        not null,  /* 	i¼i³®¼Ìj                    */
	NM_HIN_HYOKIMEI                 nvarchar        (60)        not null,  /* 	i¼i\L¼j                      */
	NM_HIN_KANA                     nvarchar        (80)        not null,  /* 	i¼Ji                            */
	NM_HIN_KANA_HANKAKU             nvarchar        (40)        not null,  /* 	i¼Ji¼p                        */
	NM_HIN_RYAKUSHO                 nvarchar        (16)        not null,  /* 	i¼ªÌ                            */
	CD_KAIHATSU_HIMBAN              nchar           (9)         not null,  /* 	J­iÔ                            */
	NM_POS_TOROKU_HIMMOKU           nvarchar        (24)        not null,  /* 	i`mAio@onro^i¼        */
	ST_HIMMOKU_TYPE                 nchar           (2)         not null,  /* 	iÚ^Cv                          */
	ST_BUNRUI_DAI                   nchar           (2)         not null,  /* 	åªÞ                              */
	ST_BUNRUI_CHU                   nchar           (2)         not null,  /* 	ªÞ                              */
	ST_BUNRUI_SHO                   nchar           (2)         not null,  /* 	¬ªÞ                              */
	ST_HINKAN_KANRI                 nchar           (2)         not null,  /* 	\¦¼ÌiiÇÇæªj            */
	ST_HIMMOKU_KANRI                nchar           (3)         not null,  /* 	iÚpÇæª                      */
	CD_SHUYAKU_HIMBAN               nchar           (9)         not null,  /* 	WñiÔ                            */
	NM_SHUYAKU_HIN                  nvarchar        (60)        not null,  /* 	Wñi¼                            */
	ST_NB_PB                        nchar           (1)         not null,  /* 	ma^oaæª                      */
	NM_PB_TOKUISAKI                 nchar           (6)         not null,  /* 	oa¾ÓæR[h                    */
	ST_REGI                         nchar           (2)         not null,  /* 	Wæªij                    */
	PR_HYOJUN_KOURI_HONTAI          numeric         (11,2)      ,          /* 	W¬¿ii{Ì¿ij            */
	PR_HYOJUN_KOURI_ZEIKOMI         numeric         (11,2)      ,          /* 	W¬¿iiÅj                */
	DT_JUCHU_KAISHI                 nchar           (8)         not null,  /* 	óJnú                          */
	DT_HAMBAI_KAISHI                nchar           (8)         not null,  /* 	ÌJnú                          */
	DT_HAMBAI_SHURYO                nchar           (8)         not null,  /* 	ÌI¹ú                          */
	DT_JUCHU_SHURYO                 nchar           (8)         not null,  /* 	óI¹ú                          */
	DT_HEMPIN_SHURYO                nchar           (8)         not null,  /* 	ÔiI¹ú                          */
	ST_HIMMOKU                      nchar           (1)         not null,  /* 	iÚXe[^X                      */
	ST_SENTAKU                      nchar           (1)         not null,  /* 	Áï^Ü¡Iðæª                  */
	ST_NISSU_JIKAN_SHOHI            nchar           (1)         not null,  /* 	úÔ\¦æªiÁï^Ü¡j      */
	ST_NISSU_JIKAN_SHIYOU           nchar           (1)         not null,  /* 	úÔ\¦æªigpúÀj        */
	ST_NISSU_JIKAN_HANBAI           nchar           (1)         not null,  /* 	úÔ\¦æªiÌúÀj        */
	DT_SHOMI_KIGEN_REITO            numeric         (4)         ,          /* 	âÜ¡úÀ                        */
	DT_SHOHI_KIGEN_KAKI             numeric         (4)         ,          /* 	ÁïúÀ^Ü¡úÀiÄGj          */
	DT_SHOHI_KIGEN_TOKI             numeric         (4)         ,          /* 	ÁïúÀ^Ü¡úÀi~Gj          */
	DT_SHIYO_KIGEN_KAKI             numeric         (4)         ,          /* 	gpúÀiÄGj                    */
	DT_SHIYO_KIGEN_TOKI             numeric         (4)         ,          /* 	gpúÀi~Gj                    */
	DT_HAMBAI_KIGEN_KAKI            numeric         (4)         ,          /* 	ÌúÀiÄGj                    */
	DT_HAMBAI_KIGEN_TOKI            numeric         (4)         ,          /* 	ÌúÀi~Gj                    */
	DT_KAKI_KAISHI                  nchar           (4)         not null,  /* 	ÄGJnú                          */
	DT_KAKI_SHURYO                  nchar           (4)         not null,  /* 	ÄGI¹ú                          */
	DT_TOKI_KAISHI                  nchar           (4)         not null,  /* 	~GJnú                          */
	DT_TOKI_SHURYO                  nchar           (4)         not null,  /* 	~GI¹ú                          */
	DT_UKEIRE_NATSU_FROM            numeric         (4)         ,          /* 	óüÂ\úiÄGjFROM              */
	DT_UKEIRE_NATSU_TO              numeric         (4)         ,          /* 	óüÂ\úiÄGjTO                */
	DT_UKEIRE_FUYU_FROM             numeric         (4)         ,          /* 	óüÂ\úi~GjFROM              */
	DT_UKEIRE_FUYU_TO               numeric         (4)         ,          /* 	óüÂ\úi~GjTO                */
	ST_RYUTSU_ONDOTAI               nchar           (1)         not null,  /* 	¬Ê·xÑ                          */
	ST_HAMBAI_ONDOTAI               nchar           (1)         not null,  /* 	Ì·xÑ                          */
	IF_KAKAKU_HYOJI                 nvarchar        (60)        not null,  /* 	¿i\¦îñ                        */
	QT_SET                          numeric         (9)         ,          /* 	Zbgiüj                    */
	CD_TORIATSUKAI_TANI             nvarchar        (3)         not null,  /* 	æµPÊ                            */
	CD_HYOJUN_TANI                  nchar           (3)         not null,  /* 	WPÊ                            */
	CD_SET_TANI                     nchar           (3)         not null,  /* 	ZbgiüjPÊ                */
	NU_HYOJUN_TORIATSUKAI_KEISU     numeric         (13,6)      ,          /* 	WæµPÊÏ·W                */
	CD_KOURI_TANI                   nvarchar        (3)         not null,  /* 	¬PÊ                            */
	NU_KOURI_HENKAN_KEISU           numeric         (6,1)       ,          /* 	¬Ï·W                        */
	CD_HEMPIN_TANI                  nvarchar        (3)         not null,  /* 	ÔiPÊ                            */
	NU_HEMPIN_HENKAN_KEISU          numeric         (6,1)       ,          /* 	ÔiÏ·W                        */
	CD_HATCHU_TANI                  nvarchar        (3)         not null,  /* 	­PÊ                            */
	NU_HATCHU_HENKAN_KEISU          numeric         (6,1)       ,          /* 	­Ï·W                        */
	NU_HYOJUN_HATCHU_KEISU          numeric         (13,6)      ,          /* 	W­PÊÏ·W                */
	QT_HATCHU_IRISU                 numeric         (9,2)       ,          /* 	­ü                            */
	CD_HATCHU_IRISU_TANI            nchar           (3)         not null,  /* 	­üPÊ                        */
	ST_FUTEIKAN                     nchar           (1)         not null,  /* 	sèÑæª                          */
	PR_OROSHI                       numeric         (11,2)      ,          /* 	µ¿iiÅ²«j                    */
	CD_SEIZO_KAISHA                 nchar           (6)         not null,  /* 	»¢ïÐR[h                      */
	CD_SEIZO_KAISHA_2               nchar           (6)         not null,  /* 	»¢ïÐR[hQ                    */
	NU_SEIZO_KAISU                  numeric         (2)         ,          /* 	»¢ñ                            */
	PR_TEMPONAI_FURIKAE             numeric         (11,2)      ,          /* 	XÜàUÖP¿                      */
	PR_TEMPOKAN_FURIKAE             numeric         (11,2)      ,          /* 	XÜÔUÖP¿                      */
	PR_EIGYOWATASHI_1               numeric         (11,2)      ,          /* 	cÆnµ¿iPiàP¿j          */
	PR_EIGYOWATASHI_2               numeric         (11,2)      ,          /* 	cÆnµ¿iQiàP¿j          */
	PR_EIGYOWATASHI_3               numeric         (11,2)      ,          /* 	cÆnµ¿iRiàP¿j          */
	PR_EIGYOWATASHI_4               numeric         (11,2)      ,          /* 	cÆnµ¿iSiàP¿j          */
	PR_EIGYOWATASHI_5               numeric         (11,2)      ,          /* 	cÆnµ¿iTiàP¿j          */
	PR_DOITSU_COMPANY_KOJOKAN       numeric         (11,2)      ,          /* 	¯êJpj[HêÔ¿i            */
	PR_BETSU_COMPANY_KOJOKAN        numeric         (11,2)      ,          /* 	ÊJpj[HêÔ¿i              */
	PR_SURIFUTO_B                   numeric         (11,2)      ,          /* 	XtgB¿i                       */
	PR_SEISAN_SHIIRE                numeric         (11,2)      ,          /* 	¶YP¿^dü¿i                  */
	PR_AQS_HYOJUN_TANKA             numeric         (11,2)      ,          /* 	AQSWP¿                         */
	ST_UMPAN_YOKI_SHUBETSU          nchar           (1)         not null,  /* 	^ÀeííÊ                        */
	QT_UMPAN_YOKI_HAKO_IRISU        numeric         (9)         ,          /* 	^Àeí ü                      */
	CD_JAN                          nchar           (15)        not null,  /* 	JANR[h                           */
	CD_BUTSURYU_KANRI               nchar           (15)        not null,  /* 	¨¬ÇR[hihsej          */
	CD_KOSEIJO_SURYO_TANI           nvarchar        (8)         not null,  /* 	\¬ãÊPÊ                      */
	CD_KOSEIJO_JURYO_TANI           nvarchar        (8)         not null,  /* 	\¬ãdÊPÊ                      */
	CD_KOSEIJO_YORYO_TANI           nvarchar        (8)         not null,  /* 	\¬ãeÊPÊ                      */
	CD_KOSEIJO_NAGASA_TANI          nchar           (8)         not null,  /* 	\¬ã·³PÊ                      */
	NU_KOSEIJO_SURYO_HENKAN_KEISU   numeric         (13,6)      ,          /* 	\¬ãÊdÊÏ·W              */
	NU_KOSEIJO_JURYO_HENKAN_KEISU   numeric         (13,6)      ,          /* 	\¬ãdÊeÊÏ·W              */
	NU_KOSEIJO_SURYO_NAGASA_KEISU   numeric         (13,6)      ,          /* 	\¬ãÊ·³Ï·W              */
	NU_KOSEIJO_JURYO_NAGASA_KEISU   numeric         (13,6)      ,          /* 	\¬ãdÊ·³Ï·W              */
	CD_GENKA_KEISAN_TANI            nchar           (3)         not null,  /* 	´¿vZP¿PÊ                    */
	NU_HYOJUN_GENKA_KEISU           numeric         (13,6)      ,          /* 	W´¿vZPÊÏ·W            */
	NU_HYOJUN_KOSEIJO_SURYO_KEISU   numeric         (13,6)      ,          /* 	W\¬ãÊPÊÏ·W          */
	NU_HYOJUN_KOSEIJO_JURYO_KEISU   numeric         (13,6)      ,          /* 	W\¬ãdÊPÊÏ·W          */
	NU_HYOJUN_KOSEIJO_YORYO_KEISU   numeric         (13,6)      ,          /* 	W\¬ãeÊPÊÏ·W          */
	NU_HYOJUN_KOSEIJO_NAGASA_KEISU  numeric         (13,6)      ,          /* 	W\¬ã·³PÊÏ·W          */
	ST_TOMEGATA                     nchar           (1)         not null,  /* 	g^æª                          */
	IF_NISUGATA                     nvarchar        (20)        not null,  /* 	×pi[ipj                      */
	CD_GENSANKOKU                   nchar           (3)         not null,  /* 	´YR[h                        */
	ST_SOBAHIN                      nchar           (1)         not null,  /* 	êiæª                          */
	ST_LOCALHIN                     nchar           (1)         not null,  /* 	[Jiæª                      */
	PR_KG                           numeric         (11,2)      ,          /* 	KGP¿                              */
	IF_ZAISHITSU                    nvarchar        (10)        not null,  /* 	Þ¿iïÞÌÞ¿j                  */
	NU_NAGASA                       numeric         (9,2)       ,          /* 	·³                                */
	NU_NAGASA_JOGEN                 numeric         (9,2)       ,          /* 	·³iãÀj                        */
	NU_NAGASA_KAGEN                 numeric         (9,2)       ,          /* 	·³iºÀj                        */
	CD_NAGASA_TANI                  nvarchar        (8)         not null,  /* 	·³PÊ                            */
	NU_HABA                         numeric         (9,2)       ,          /* 	                                  */
	NU_HABA_JOGEN                   numeric         (9,2)       ,          /* 	iãÀj                          */
	NU_HABA_KAGEN                   numeric         (9,2)       ,          /* 	iºÀj                          */
	CD_HABA_TANI                    nvarchar        (8)         not null,  /* 	PÊ                              */
	NU_TAKASA                       numeric         (9,2)       ,          /* 	³iú³j                        */
	NU_TAKASA_JOGEN                 numeric         (9,2)       ,          /* 	³iú³jiãÀj                */
	NU_TAKASA_KAGEN                 numeric         (9,2)       ,          /* 	³iú³jiºÀj                */
	CD_TAKASA_TANI                  nchar           (3)         not null,  /* 	³iú³jPÊ                    */
	NU_JURYO                        numeric         (9,2)       ,          /* 	dÊ                                */
	NU_JURYO_JOGEN                  numeric         (9,2)       ,          /* 	dÊiãÀj                        */
	NU_JURYO_KAGEN                  numeric         (9,2)       ,          /* 	dÊiºÀj                        */
	NU_SOJURYO                      numeric         (9,2)       ,          /* 	dÊ                              */
	CD_JURYO_TANI                   nvarchar        (8)         not null,  /* 	dÊPÊ                            */
	NU_SOSO                         numeric         (3)         ,          /* 	w                              */
	NU_TSUTSU_CHOKKEI               numeric         (9,2)       ,          /* 	¼a                              */
	NU_SAIDAI_CHOKKEI               numeric         (9,2)       ,          /* 	Åå¼a                            */
	ST_MAKI_HOKO                    nchar           (3)         not null,  /* 	ª«ûü                            */
	CD_FUKURO_IRISU_TANI            nchar           (3)         not null,  /* 	ÜüPÊ                          */
	QT_FUKURO_IRISU                 numeric         (9)         ,          /* 	Üü                              */
	IF_HAMBAIYO_HOZAI               nvarchar        (100)       not null,  /* 	ÌpïÞ                          */
	FG_SHOMI_SHOHIKIGEN_YO_FUYO     nchar           (1)         not null,  /* 	Ü¡^ÁïúÀ\¦@vsv          */
	FG_GENZAIRYO_YO_FUYO            nchar           (1)         not null,  /* 	´Þ¿\¦@vsv                  */
	ST_SENTAKU_100G                 nchar           (3)         not null,  /* 	h{¬ª\¦PÊIðæªi100g)    */
	FG_EIYOSEIBUN_HYOJI             nchar           (1)         not null,  /* 	h{¬ª\¦@vsv                */
	IF_SHIKIBETSU_HYOJI             nvarchar        (100)       not null,  /* 	¯Ê\¦ij                      */
	IF_PRICECARD_HYOJI_NAIYO        nvarchar        (100)       not null,  /* 	vCXJ[h\¦àe              */
	IF_PRICECARD_GAZO_1             nvarchar        (100)       not null,  /* 	vCXJ[hpæP              */
	IF_PRICECARD_GAZO_2             nvarchar        (100)       not null,  /* 	vCXJ[hpæQ              */
	IF_PRICECARD_GAZO_3             nvarchar        (100)       not null,  /* 	vCXJ[hpæR              */
	NU_NAIYO_JURYO                  numeric         (9,2)       ,          /* 	àeÊdÊ                          */
	CD_NAIYO_JURYO                  nvarchar        (8)         not null,  /* 	àeÊdÊPÊ                      */
	NU_KOKEI_JURYO                  numeric         (9,2)       ,          /* 	Å`dÊ                            */
	CD_KEKEI_JURYO                  nvarchar        (8)         not null,  /* 	Å`dÊPÊ                        */
	NU_HOKAN_JOKEN_KAIFU_MAE        nchar           (1)         not null,  /* 	ÛÇðJOú                  */
	NU_HOKAN_JOKEN_KAIFU_GO         nchar           (1)         not null,  /* 	ÛÇðJãú                  */
	DT_KAIFUGO_SHIYOKIGEN_KAKI      nchar           (8)         not null,  /* 	JãgpúÀiÄGj              */
	ST_KAIFUGO_SHIYOKIGEN_KAKI      nchar           (1)         not null,  /* 	JãgpúÀPÊiÄGj          */
	DT_KAIFUGO_SHIYOKIGEN_TOKI      nchar           (8)         not null,  /* 	JãgpúÀi~Gj              */
	ST_KAIFUGO_SHIYOKIGEN_TOKI      nchar           (1)         not null,  /* 	JãgpúÀPÊi~Gj          */
	CD_YAKIIRO_NERAISHOKU           nchar           (8)         not null,  /* 	Ä«FiËç¢Fj                  */
	CD_YAKIIRO_KAGEN                nchar           (8)         not null,  /* 	Ä«FºÀ                          */
	CD_YAKIIRO_JOGEN                nchar           (8)         not null,  /* 	Ä«FãÀ                          */
	CD_SAISHO_SEZO_TANI             nchar           (3)         not null,  /* 	Å¬»¢PÊPÊ                  */
	QT_SAISHO_SEIZO_TANI            numeric         (2)         ,          /* 	Å¬»¢PÊ                      */
	FG_RITADO                       nchar           (1)         not null,  /* 	^[hÂÛ                        */
	CD_SAIJI                        nchar           (5)         not null,  /* 	ÃR[h                          */
	ST_SHOHIN_SEIHIN                nchar           (1)         not null,  /* 	¤i»iæª                        */
	ST_KEIHI_HINSHU                 nchar           (1)         not null,  /* 	oïiíæª                        */
	ST_SHIIRE                       nchar           (1)         not null,  /* 	düæª                            */
	ST_SHOHIN_BUNRUI                nchar           (3)         not null,  /* 	¤iªÞ                            */
	FG_GENTEI_UMU                   nchar           (1)         not null,  /* 	ÀèL³                            */
	FG_WEB_JYUCHU_KANRI             nchar           (1)         not null,  /* 	WebóÇæª                     */
	NU_ZEIRITSU                     numeric         (3,1)       ,          /* 	Å¦                                */
	ST_ZEIRITSU                     nchar           (1)         not null,  /* 	Å¦æªmÁïÅn                  */
	FG_ZAIKO_KANRI                  nchar           (1)         not null,  /* 	ÝÉÇtO                      */
	ST_GR                           nchar           (1)         not null,  /* 	GRæª                              */
	ST_HASU_SHOHIZEI                nchar           (1)         not null,  /* 	[æªmÁïÅn                  */
	ST_KANRI_HIMMOKU                nchar           (1)         not null,  /* 	ÇiÚæª                        */
	CD_OYAHIMBAN                    nchar           (9)         not null,  /* 	eiÔ                              */
	ST_KAZEI                        nchar           (1)         not null,  /* 	ÛÅæªmÁïÅn                  */
	ST_URIAGE_BUNRUI                nchar           (2)         not null,  /* 	ãªÞ                            */
	ST_DAI                          nchar           (3)         not null,  /* 	¤iæªå                          */
	ST_CHU                          nchar           (1)         not null,  /* 	¤iæª                          */
	ST_SHO                          nchar           (2)         not null,  /* 	¤iæª¬                          */
	ST_HIMMOKU_BUNRUI               nchar           (1)         not null,  /* 	iÚªÞ                            */
	ST_NOHIMBI_FUTEI                nchar           (1)         not null,  /* 	[iúsèæª                      */
	ST_EIGYO_URIAGE                 nchar           (2)         not null,  /* 	cÆãªÞ                        */
	ST_EIGYO_KANRI                  nchar           (1)         not null,  /* 	cÆÇæª                        */
	ST_SEIHIN_SHOHIN                nchar           (1)         not null,  /* 	»i¤iæª                        */
	ST_SHOHIN_CATEGORY              nchar           (1)         not null,  /* 	¤iJeS                        */
	ST_KIJI_SHURUI                  nchar           (1)         not null,  /* 	¶níÞ                            */
	QT_BARA_TANI                    nvarchar        (3)         not null,  /* 	oPÊ                            */
	NU_HYOJUN_BARA_KEISU            numeric         (13,6)      ,          /* 	WoPÊÏ·W                */
	CD_RETAIL_HINGUN                nchar           (4)         not null,  /* 	eCiQ                        */
	CD_TANTO_BUMON                  nchar           (4)         not null,  /* 	Så                            */
	NU_HAMBAI_NISSU                 numeric         (3)         ,          /* 	Ìú                            */
	CD_HYOJUN_SHIIRESAKI            nvarchar        (5)         not null,  /* 	WdüæR[h                    */
	PR_HYOJUN_TANKA                 numeric         (11,2)      ,          /* 	WdüP¿                        */
	PR_RIRON_GENKA                  numeric         (11,2)      ,          /* 	_´¿                            */
	PR_HYOJUN_GENKA                 numeric         (11,2)      ,          /* 	W´¿                            */
	PR_SEIZO_GENKA                  numeric         (11,2)      ,          /* 	»¢´¿                            */
	ST_KOJO                         nchar           (1)         not null,  /* 	Hêæª                            */
	ST_RETAIL                       nchar           (1)         not null,  /* 	eCæª                        */
	ST_OROSHI                       nchar           (1)         not null,  /* 	µæª                              */
	ST_HIMMOKU_KAZEI                nchar           (1)         not null,  /* 	iÚÛÅæªioj                */
	ST_CASE                         nchar           (2)         not null,  /* 	P[Xæª                          */
	IF_HIMMOKU_GAZO_1               nvarchar        (100)       not null,  /* 	iÚæP                          */
	IF_HIMMOKU_GAZO_2               nvarchar        (100)       not null,  /* 	iÚæQ                          */
	IF_HIMMOKU_GAZO_3               nvarchar        (100)       not null,  /* 	iÚæR                          */
	IF_HIMMOKU_GAZO_4               nvarchar        (100)       not null,  /* 	iÚæS                          */
	CD_SHOHIN_BUNRUI_9              nchar           (10)        not null,  /* 	¤iªÞX                          */
	DT_ZENKAI_HENKO                 nchar           (8)         not null,  /* 	OñÏXú                          */
	DT_OYA_KAISHI                   nchar           (8)         not null,  /* 	eJnú                            */
	DT_HATSUBAI_SHOUNIN             nchar           (8)         not null,  /* 	­³Fú                          */
	FG_SHOUHINSEKKEIIRAISHO_KAHI    nchar           (1)         not null,  /* 	¤iÝvËoÍÂÛ              */
	FG_SHOUHINSEKKEISHO_KAHI        nchar           (1)         not null,  /* 	¤iÝvoÍÂÛ                  */
	FG_KAIHATSUSHOUNINSHO_KAHI      nchar           (1)         not null,  /* 	J­³FoÍÂÛ                  */
	FG_HATSUBAISHOUNINSHO_KAHI      nchar           (1)         not null,  /* 	­³FoÍÂÛ                  */
	FG_HOUZAIIRAISHO_KAHI           nchar           (1)         not null,  /* 	ïÞËoÍÂÛ                  */
	FG_HINMOKUTOUROKUIRAISHO_KAHI   nchar           (1)         not null,  /* 	iÚo^ËoÍÂÛ              */
	CD_PROCESS                      nvarchar        (20)        not null,  /* 	vZXR[h                      */
	ST_BUTSURYU_SEIGYO              nchar           (3)         not null,  /* 	¨¬§äæª                        */
	CD_KANRI_COMPANY                nchar           (5)         not null,  /* 	ÇJpj[R[h                */
	IF_SHIKIBETSU_HYOJI_PLA         nvarchar        (100)       not null,  /* 	¯Ê\¦(vX`bN)             */
	DT_RENKEI                       nchar           (14)        not null,  /* 	Agú                            */
	NU_HAITA                        numeric         (5)         not null,  /* 	r¼JE^                        */
	CD_ITEM_GTIN					          nchar           (15)        not null,  /*  GTIN                                */

	createdAt datetime not null,		/*	f[^}üú				*/
	updatedAt datetime not null,		/*	f[^XVú				*/

	constraint pk_TB_MHIMMOKU primary key (CD_HIMBAN,DT_TEKIYOKAISHI)
);
