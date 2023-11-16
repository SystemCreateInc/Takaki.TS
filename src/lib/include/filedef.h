/*
 *	ファイルレイアウト定義
 *	
 *	Copyright:	(c) 2011 System Create Inc.
 *
 *
 */

#ifndef __FILEDEF_H__
#define __FILEDEF_H__



/*
 *	ピッキングデータレコード
 *
 *	filename:syuk.txt(514 Byte)
 *	entry:
 *	type:		text
 *	unique:		
*/

/*
 *	ピッキングデータ
 */ 
typedef	struct {
	char	shipdt[8];					/*	出荷日											*/
	char	logisticsgr[5];				/*	物流グループコード								*/
	char	chain[7];					/*	チェーンコード									*/
	char	chainnm[20];				/*	チェーン名称									*/
	char	shop[7];					/*	店舗コード										*/
	char	shopnm[20];					/*	店舗名称										*/
	char	packno[36];					/*	梱包No											*/
	char	rackno[6];					/*	棚番											*/
	char	item[14];					/*	商品コード										*/
	char	itemnm[40];					/*	商品名称										*/
	char	delivdt[6];					/*	納品日											*/
	char	workno[7];					/*	作業No											*/
	char	sstore[6];					/*	Ｓ売場コード									*/
	char	pickseq[5];					/*	ピッキングＳＥＱ								*/
	char	course[5];					/*	コース											*/
	char	route[3];					/*	路順											*/
	char	slipno[10];					/*	伝票ナンバー									*/
	char	slipnocd[1];				/*	伝票ナンバーコード								*/
	char	sliprow[3];					/*	伝票行											*/
	char	ops[7];						/*	受注数											*/
	char	dops[7];					/*	出荷予定数										*/
	char	drps[7];					/*	出荷実績数										*/
	char	plandt[8];					/*	予定作成日										*/
	char	plantm[6];					/*	予定作成時刻									*/
	char	resultdt[8];				/*	実績作成日										*/
	char	resulttm[6];				/*	実績作成時刻									*/
	char	person[7];					/*	担当者ＣＤ										*/
	char	losstype[1];				/*	出荷時欠品フラグ 1：あり ﾌﾞﾗﾝｸ：なし			*/
	char	boxappend[1];				/*	箱追加フラグ 1：あり ﾌﾞﾗﾝｸ：なし				*/
	char	workkey[20];				/*	作業キー 作業ﾅﾝﾊﾞｰ＋店舗ｺｰﾄﾞ＋S 売場ｺｰﾄﾞ		*/
	char	csunit[7];					/*	出荷入数										*/
	char	csunittype[1];				/*	出荷入数制限区分 1：あり ﾌﾞﾗﾝｸ：なし			*/
	char	crlf[2];					/*	改行											*/
} F_DIST;


#endif //__FILEDEF_H__
