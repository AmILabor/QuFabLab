from moodle_api.models.custom_db_model import CustomDBModel
from django.db import models

from moodle_api.models.mdl_question import MdlQuestion


class MdlQuestionAnswers(CustomDBModel):
    id = models.BigAutoField(primary_key=True)
    question = models.ForeignKey(db_column="question",related_name="answers",to=MdlQuestion, on_delete=models.DO_NOTHING)
    answer = models.TextField()
    answerformat = models.IntegerField()
    fraction = models.DecimalField(max_digits=12, decimal_places=7)
    feedback = models.TextField()
    feedbackformat = models.IntegerField()

    class Meta:
        managed = False
        db_table = 'mdl_question_answers'
