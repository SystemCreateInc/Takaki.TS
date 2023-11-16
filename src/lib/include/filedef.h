/*
 *	�t�@�C�����C�A�E�g��`
 *	
 *	Copyright:	(c) 2011 System Create Inc.
 *
 *
 */

#ifndef __FILEDEF_H__
#define __FILEDEF_H__



/*
 *	�s�b�L���O�f�[�^���R�[�h
 *
 *	filename:syuk.txt(514 Byte)
 *	entry:
 *	type:		text
 *	unique:		
*/

/*
 *	�s�b�L���O�f�[�^
 */ 
typedef	struct {
	char	shipdt[8];					/*	�o�ד�											*/
	char	logisticsgr[5];				/*	�����O���[�v�R�[�h								*/
	char	chain[7];					/*	�`�F�[���R�[�h									*/
	char	chainnm[20];				/*	�`�F�[������									*/
	char	shop[7];					/*	�X�܃R�[�h										*/
	char	shopnm[20];					/*	�X�ܖ���										*/
	char	packno[36];					/*	����No											*/
	char	rackno[6];					/*	�I��											*/
	char	item[14];					/*	���i�R�[�h										*/
	char	itemnm[40];					/*	���i����										*/
	char	delivdt[6];					/*	�[�i��											*/
	char	workno[7];					/*	���No											*/
	char	sstore[6];					/*	�r����R�[�h									*/
	char	pickseq[5];					/*	�s�b�L���O�r�d�p								*/
	char	course[5];					/*	�R�[�X											*/
	char	route[3];					/*	�H��											*/
	char	slipno[10];					/*	�`�[�i���o�[									*/
	char	slipnocd[1];				/*	�`�[�i���o�[�R�[�h								*/
	char	sliprow[3];					/*	�`�[�s											*/
	char	ops[7];						/*	�󒍐�											*/
	char	dops[7];					/*	�o�ח\�萔										*/
	char	drps[7];					/*	�o�׎��ѐ�										*/
	char	plandt[8];					/*	�\��쐬��										*/
	char	plantm[6];					/*	�\��쐬����									*/
	char	resultdt[8];				/*	���э쐬��										*/
	char	resulttm[6];				/*	���э쐬����									*/
	char	person[7];					/*	�S���҂b�c										*/
	char	losstype[1];				/*	�o�׎����i�t���O 1�F���� ���ݸ�F�Ȃ�			*/
	char	boxappend[1];				/*	���ǉ��t���O 1�F���� ���ݸ�F�Ȃ�				*/
	char	workkey[20];				/*	��ƃL�[ ������ް�{�X�ܺ��ށ{S ���꺰��		*/
	char	csunit[7];					/*	�o�ד���										*/
	char	csunittype[1];				/*	�o�ד��������敪 1�F���� ���ݸ�F�Ȃ�			*/
	char	crlf[2];					/*	���s											*/
} F_DIST;


#endif //__FILEDEF_H__
