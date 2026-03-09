from django.db import models

from moodle_api.models.custom_db_model import CustomDBModel


class MdlLesson(CustomDBModel):
    id = models.BigAutoField(primary_key=True)
    course = models.BigIntegerField()
    name = models.CharField(max_length=255)
    intro = models.TextField(blank=True, null=True)
    introformat = models.SmallIntegerField()
    practice = models.SmallIntegerField()
    modattempts = models.SmallIntegerField()
    usepassword = models.SmallIntegerField()
    password = models.CharField(max_length=32)
    dependency = models.BigIntegerField()
    conditions = models.TextField()
    grade = models.BigIntegerField()
    custom = models.SmallIntegerField()
    ongoing = models.SmallIntegerField()
    usemaxgrade = models.SmallIntegerField()
    maxanswers = models.SmallIntegerField()
    maxattempts = models.SmallIntegerField()
    review = models.SmallIntegerField()
    nextpagedefault = models.SmallIntegerField()
    feedback = models.SmallIntegerField()
    minquestions = models.SmallIntegerField()
    maxpages = models.SmallIntegerField()
    timelimit = models.BigIntegerField()
    retake = models.SmallIntegerField()
    activitylink = models.BigIntegerField()
    mediafile = models.CharField(max_length=255)
    mediaheight = models.BigIntegerField()
    mediawidth = models.BigIntegerField()
    mediaclose = models.SmallIntegerField()
    slideshow = models.SmallIntegerField()
    width = models.BigIntegerField()
    height = models.BigIntegerField()
    bgcolor = models.CharField(max_length=7)
    displayleft = models.SmallIntegerField()
    displayleftif = models.SmallIntegerField()
    progressbar = models.SmallIntegerField()
    available = models.BigIntegerField()
    deadline = models.BigIntegerField()
    timemodified = models.BigIntegerField()
    completionendreached = models.IntegerField(blank=True, null=True)
    completiontimespent = models.BigIntegerField(blank=True, null=True)
    allowofflineattempts = models.IntegerField(blank=True, null=True)

    class Meta:
        managed = False
        db_table = 'mdl_lesson'


class MdlLessonAnswers(CustomDBModel):
    id = models.BigAutoField(primary_key=True)
    lessonid = models.BigIntegerField()
    pageid = models.BigIntegerField()
    jumpto = models.BigIntegerField()
    grade = models.SmallIntegerField()
    score = models.BigIntegerField()
    flags = models.SmallIntegerField()
    timecreated = models.BigIntegerField()
    timemodified = models.BigIntegerField()
    answer = models.TextField(blank=True, null=True)
    answerformat = models.IntegerField()
    response = models.TextField(blank=True, null=True)
    responseformat = models.IntegerField()

    class Meta:
        managed = False
        db_table = 'mdl_lesson_answers'


class MdlLessonAttempts(CustomDBModel):
    id = models.BigAutoField(primary_key=True)
    lessonid = models.BigIntegerField()
    pageid = models.BigIntegerField()
    userid = models.BigIntegerField()
    answerid = models.BigIntegerField()
    retry = models.SmallIntegerField()
    correct = models.BigIntegerField()
    useranswer = models.TextField(blank=True, null=True)
    timeseen = models.BigIntegerField()

    class Meta:
        managed = False
        db_table = 'mdl_lesson_attempts'


class MdlLessonBranch(CustomDBModel):
    id = models.BigAutoField(primary_key=True)
    lessonid = models.BigIntegerField()
    userid = models.BigIntegerField()
    pageid = models.BigIntegerField()
    retry = models.BigIntegerField()
    flag = models.SmallIntegerField()
    timeseen = models.BigIntegerField()
    nextpageid = models.BigIntegerField()

    class Meta:
        managed = False
        db_table = 'mdl_lesson_branch'


class MdlLessonGrades(CustomDBModel):
    id = models.BigAutoField(primary_key=True)
    lessonid = models.BigIntegerField()
    userid = models.BigIntegerField()
    grade = models.FloatField()
    late = models.SmallIntegerField()
    completed = models.BigIntegerField()

    class Meta:
        managed = False
        db_table = 'mdl_lesson_grades'


class MdlLessonOverrides(CustomDBModel):
    id = models.BigAutoField(primary_key=True)
    lessonid = models.BigIntegerField()
    groupid = models.BigIntegerField(blank=True, null=True)
    userid = models.BigIntegerField(blank=True, null=True)
    available = models.BigIntegerField(blank=True, null=True)
    deadline = models.BigIntegerField(blank=True, null=True)
    timelimit = models.BigIntegerField(blank=True, null=True)
    review = models.SmallIntegerField(blank=True, null=True)
    maxattempts = models.SmallIntegerField(blank=True, null=True)
    retake = models.SmallIntegerField(blank=True, null=True)
    password = models.CharField(max_length=32, blank=True, null=True)

    class Meta:
        managed = False
        db_table = 'mdl_lesson_overrides'


class MdlLessonPages(CustomDBModel):
    id = models.BigAutoField(primary_key=True)
    lessonid = models.ForeignKey(db_column="lessonid",related_name="pages",to=MdlLesson,on_delete=models.DO_NOTHING)
    prevpageid = models.BigIntegerField()
    nextpageid = models.BigIntegerField()
    qtype = models.SmallIntegerField()
    qoption = models.SmallIntegerField()
    layout = models.SmallIntegerField()
    display = models.SmallIntegerField()
    timecreated = models.BigIntegerField()
    timemodified = models.BigIntegerField()
    title = models.CharField(max_length=255)
    contents = models.TextField()
    contentsformat = models.IntegerField()

    class Meta:
        managed = False
        db_table = 'mdl_lesson_pages'
