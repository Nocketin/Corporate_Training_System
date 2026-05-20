import type { CourseDetail } from '../../../api/courses';
import type { ModuleDraft, ResourceDraft } from './types';

export function courseDetailToDraft(course: CourseDetail): ModuleDraft[] {
  return [...course.modules]
    .sort((a, b) => a.order - b.order)
    .map((module, moduleIndex) => ({
      clientId: crypto.randomUUID(),
      serverId: module.id,
      title: module.title,
      order: moduleIndex,
      lessons: [...module.lessons]
        .sort((a, b) => a.order - b.order)
        .map((lesson, lessonIndex) => ({
          clientId: crypto.randomUUID(),
          serverId: lesson.id,
          title: lesson.title,
          textContent: lesson.textContent ?? '',
          durationMinutes: lesson.durationMinutes,
          order: lessonIndex,
          resources: [...lesson.resources]
            .filter((resource) => resource.id !== '00000000-0000-0000-0000-000000000000')
            .sort((a, b) => a.order - b.order)
            .map((resource, resourceIndex): ResourceDraft => {
              if (resource.kind.toLowerCase() === 'file') {
                return {
                  clientId: crypto.randomUUID(),
                  kind: 'file',
                  title: resource.title,
                  order: resourceIndex,
                  file: null,
                  existingFileName: resource.fileName ?? undefined,
                  serverId: resource.id,
                };
              }

              return {
                clientId: crypto.randomUUID(),
                kind: 'link',
                title: resource.title,
                url: resource.url ?? '',
                order: resourceIndex,
                serverId: resource.id,
              };
            }),
        })),
    }));
}
