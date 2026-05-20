export type LinkResourceDraft = {
  clientId: string;
  kind: 'link';
  title: string;
  url: string;
  order: number;
  serverId?: string;
};

export type FileResourceDraft = {
  clientId: string;
  kind: 'file';
  title: string;
  order: number;
  file: File | null;
  existingFileName?: string;
  serverId?: string;
};

export type ResourceDraft = LinkResourceDraft | FileResourceDraft;

export type LessonDraft = {
  clientId: string;
  serverId?: string;
  title: string;
  textContent: string;
  durationMinutes: number;
  order: number;
  resources: ResourceDraft[];
};

export type ModuleDraft = {
  clientId: string;
  serverId?: string;
  title: string;
  order: number;
  lessons: LessonDraft[];
};

export const createEmptyModule = (order: number): ModuleDraft => ({
  clientId: crypto.randomUUID(),
  title: '',
  order,
  lessons: [createEmptyLesson(0)],
});

export const createEmptyLesson = (order: number): LessonDraft => ({
  clientId: crypto.randomUUID(),
  title: '',
  textContent: '',
  durationMinutes: 10,
  order,
  resources: [],
});

export const createLinkResource = (order: number): LinkResourceDraft => ({
  clientId: crypto.randomUUID(),
  kind: 'link',
  title: '',
  url: '',
  order,
});

export const createFileResource = (order: number): FileResourceDraft => ({
  clientId: crypto.randomUUID(),
  kind: 'file',
  title: '',
  order,
  file: null,
});
