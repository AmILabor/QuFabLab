from django.db.models.expressions import RawSQL

from moodle_api.models.mdl_lesson import MdlLesson
from moodle_api.models.mdl_question import MdlQuestion

def get_id_for_tag_by_itemtype(tag: str, itemtype: str)->RawSQL:
    return RawSQL(
        'SELECT itemid from mdl_tag_instance as ti left join mdl_tag tag on ti.tagid = tag.id where '
        'tag.name=%s and ti.itemtype=%s',
        [tag,itemtype]
    )

def get_questions_by_tag(tag: str) -> MdlQuestion:
    return MdlQuestion.objects.filter(id__in=get_id_for_tag_by_itemtype(tag,"question"))


def get_lesson_by_tag(tag: str) -> MdlLesson:
    sql = RawSQL('select instance from mdl_course_modules where id in (select ti.itemid '
                 'from mdl_tag_instance as ti left join mdl_tag tag '
                 'on ti.tagid = tag.id where tag.name = %s and ti.itemtype=%s) and module=15',
                 [tag,"course_modules"])

    return MdlLesson.objects.filter(id__in=sql)