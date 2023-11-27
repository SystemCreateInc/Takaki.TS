/*
 *
 *	仕分グループ単位のコース順
 *
 *	$Id: TB_DIST_GROUP_COURSE.sql,v 1.1.1.1 2005/07/25 02:23:05 ohki Exp $
 */

create table TB_DIST_GROUP_COURSE (
	ID_DIST_GROUP			bigint 			 		not null,	/* id							*/
	
	NU_COURSE_SEQ			integer					not null,	/* コース表示順					*/
	CD_COURSE				nchar(3)				not null,	/* コース						*/

	createdAt 				datetime 				not null,	/*	データ挿入日				*/
	updatedAt 				datetime 				not null,	/*	データ更新日				*/
	
	constraint fk_TB_DIST_GROUP_COURSE foreign key (ID_DIST_GROUP) references TB_DIST_GROUP(ID_DIST_GROUP) on delete cascade,
	
	constraint pk_TB_DIST_GROUP_COURSE primary key (ID_DIST_GROUP,NU_COURSE_SEQ)
);

