import type { ModuleDraft } from './types';

export type CourseFormPayload = {
  title: string;
  description: string;
  authorId?: string;
  price: number;
  cover: File | null;
  modules: ModuleDraft[];
  deletedResourceIds: string[];
};

export function buildCourseFormData(payload: CourseFormPayload, mode: 'create' | 'update'): FormData {
  const form = new FormData();
  form.append('title', payload.title);
  form.append('description', payload.description);
  form.append('price', String(payload.price));

  if (mode === 'create' && payload.authorId) {
    form.append('authorId', payload.authorId);
  }

  if (payload.cover) {
    form.append('cover', payload.cover);
  }

  if (mode === 'update' && payload.deletedResourceIds.length > 0) {
    form.append('deletedResourceIds', JSON.stringify(payload.deletedResourceIds));
  }

  const modulesJson = payload.modules.map((module, moduleIndex) => ({
    id: module.serverId ?? null,
    title: module.title,
    order: moduleIndex,
    lessons: module.lessons.map((lesson, lessonIndex) => ({
      id: lesson.serverId ?? null,
      title: lesson.title,
      contentUrl: null,
      textContent: lesson.textContent || null,
      durationMinutes: lesson.durationMinutes,
      order: lessonIndex,
      resources: lesson.resources.map((resource, resourceIndex) => {
        if (resource.kind === 'link') {
          return {
            id: resource.serverId ?? null,
            kind: 'link',
            title: resource.title,
            order: resourceIndex,
            url: resource.url,
            fileKey: null,
          };
        }

        let fileKey: string | null = null;
        if (resource.file) {
          fileKey = `${module.clientId}_${lesson.clientId}_${resource.clientId}`;
        } else if (!resource.serverId) {
          fileKey = `${module.clientId}_${lesson.clientId}_${resource.clientId}`;
        }

        return {
          id: resource.serverId ?? null,
          kind: 'file',
          title: resource.title,
          order: resourceIndex,
          url: null,
          fileKey,
        };
      }),
    })),
  }));

  form.append('modulesJson', JSON.stringify(modulesJson));

  for (const module of payload.modules) {
    for (const lesson of module.lessons) {
      for (const resource of lesson.resources) {
        if (resource.kind === 'file' && resource.file) {
          const fileKey = `${module.clientId}_${lesson.clientId}_${resource.clientId}`;
          form.append(`lessonFile_${fileKey}`, resource.file);
        }
      }
    }
  }

  return form;
}
