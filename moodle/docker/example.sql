Was beweist das Doppelspaltexperiment über Licht?
Das Doppelspaltexperiment beweist, dass Licht sowohl Wellen- als auch Teilcheneigenschaften hat. Dies wird durch das Interferenzmuster auf der Leinwand gezeigt, das man nur bei Wellen, nicht aber bei Teilchen sieht.
Select one:
[ ] True
[ ] False


select * from mdl_question where id=10;

| id | parent | name          | questiontext                                                                                                                                                                                                                                                                                                                                                                                                       | questiontextformat | generalfeedback | generalfeedbackformat | defaultmark | penalty   | qtype     | length | stamp                              | timecreated | timemodified | createdby | modifiedby |
+----+--------+---------------+--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------+--------------------+-----------------+-----------------------+-------------+-----------+-----------+--------+------------------------------------+-------------+--------------+-----------+------------+
| 10 |      0 | Quantenphysik | <p dir="ltr" style="text-align: left;">Was beweist das Doppelspaltexperiment über Licht?</p><p dir="ltr" style="text-align: left;"><br></p><p dir="ltr" style="text-align: left;">Das Doppelspaltexperiment beweist, dass Licht sowohl Wellen- als auch Teilcheneigenschaften hat. Dies wird durch das Interferenzmuster auf der Leinwand gezeigt, das man nur bei Wellen, nicht aber bei Teilchen sieht.<br></p>  |                  1 |                 |                     1 |   1.0000000 | 1.0000000 | truefalse |      1 | moodle.hsnr.de+230210103618+i2z6j1 |  1676025378 |   1676026123 |         2 |          2 |

select * from mdl_question_answers where question=10;

| id | question | answer | answerformat | fraction  | feedback                                                                                                                                                                                                                                                                                            | feedbackformat |
+----+----------+--------+--------------+-----------+-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------+----------------+
| 31 |       10 | Wahr   |            0 | 1.0000000 | <p dir="ltr" style="text-align: left;"></p><div>

<p>Das Doppelspaltexperiment beweist, dass Licht
sowohl Wellen- als auch Teilcheneigenschaften hat. Dies wird durch das
Interferenzmuster auf der Leinwand gezeigt, das man nur bei Wellen, nicht aber
bei Teilchen sieht.</p>

</div><br><p></p> |              1 |
| 32 |       10 | Falsch |            0 | 0.0000000 |                                                                                                                                                                                                                                                                                                     |              1 |


# Database er model https://www.examulator.com/er/4.1/index.html
# Tag lookup
use bitnami_moodle;
select * from mdl_tag where name='doppelspalt';
select * from mdl_tag_instance where tagid=16;
select * from mdl_course_modules where id in (13,7); -- itemid
-- select * from mdl_lesson where id in (1); -- instance if module is 15
select * from mdl_quiz where id =1 ; -- instance if module is 18
-- select * from mdl_modules where id=18; -- module


-- Questions for quiz with given tag
SELECT q.id AS questionid, q.questiontext, q.name AS questionname
FROM mdl_quiz_slots slot
LEFT JOIN mdl_question_references qr ON qr.component = 'mod_quiz'
AND qr.questionarea = 'slot' AND qr.itemid = slot.id
LEFT JOIN mdl_question_bank_entries qbe ON qbe.id = qr.questionbankentryid
LEFT JOIN mdl_question_versions qv ON qv.questionbankentryid = qbe.id
LEFT JOIN mdl_question q ON q.id = qv.questionid
WHERE slot.quizid = 1;





use bitnami_moodle;
select *
from mdl_tag
where name = 'doppelspalt';
select *
from mdl_tag_instance as ti
         left join mdl_tag tag on ti.tagid = tag.id
where tag.name = 'doppelspalt';

select *
from mdl_course_modules
where id in (13, 7);
-- itemid

-- Question
select *
from mdl_question
where id in
      (select itemid
       from mdl_tag_instance as ti
                left join mdl_tag tag on ti.tagid = tag.id
       where tag.name = 'doppelspalt'
         and ti.itemtype = 'question');

-- Lesson

select * from mdl_course_modules where id in (select ti.itemid
from mdl_tag_instance as ti
         left join mdl_tag tag on ti.tagid = tag.id
where tag.name = 'interferenz' and ti.itemtype='course_modules') and module=15;

select * from mdl_lesson where id in (select instance from mdl_course_modules where id in (select ti.itemid
from mdl_tag_instance as ti
         left join mdl_tag tag on ti.tagid = tag.id
where tag.name = 'interferenz' and ti.itemtype='course_modules') and module=15);


-- instance if module is 15

-- Quiz
select *
from mdl_quiz
where id = 1;
-- instance if module is 18
-- select * from mdl_modules where id=18; -- module
SELECT q.id AS questionid, q.questiontext, q.name AS questionname
FROM mdl_quiz_slots slot
         LEFT JOIN mdl_question_references qr ON qr.component = 'mod_quiz'
    AND qr.questionarea = 'slot' AND qr.itemid = slot.id
         LEFT JOIN mdl_question_bank_entries qbe ON qbe.id = qr.questionbankentryid
         LEFT JOIN mdl_question_versions qv ON qv.questionbankentryid = qbe.id
         LEFT JOIN mdl_question q ON q.id = qv.questionid
WHERE slot.quizid = 1;

