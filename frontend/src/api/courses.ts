import { api } from './axiosInstance';

export type CourseListItem = {
  id: string;
  title: string;
  description: string;
  authorId: string;
  price: number;
  coverImageUrl: string | null;
};

export type LessonResourceDto = {
  id: string;
  kind: string;
  title: string;
  order: number;
  url: string | null;
  fileName: string | null;
  contentType: string | null;
  fileSizeBytes: number | null;
  downloadUrl: string | null;
};

export type LessonDto = {
  id: string;
  title: string;
  contentUrl: string | null;
  textContent: string | null;
  durationMinutes: number;
  order: number;
  resources: LessonResourceDto[];
};

export type ModuleDto = {
  id: string;
  title: string;
  order: number;
  lessons: LessonDto[];
};

export type CourseDetail = {
  id: string;
  title: string;
  description: string;
  authorId: string;
  price: number;
  coverImageBucket: string | null;
  coverImageKey: string | null;
  modules: ModuleDto[];
};

export async function fetchCourses(): Promise<CourseListItem[]> {
  const { data } = await api.get<CourseListItem[]>('/api/courses');
  return data;
}

export async function fetchCourse(id: string): Promise<CourseDetail> {
  const { data } = await api.get<CourseDetail>(`/api/courses/${id}`);
  return data;
}

export async function createCourse(form: FormData): Promise<CourseDetail> {
  const { data } = await api.post<CourseDetail>('/api/courses', form);
  return data;
}

export async function updateCourse(id: string, form: FormData): Promise<CourseDetail> {
  const { data } = await api.put<CourseDetail>(`/api/courses/${id}`, form);
  return data;
}

export function getLessonResourceFileUrl(lessonId: string, resourceId: string): string {
  return `/api/lessons/${lessonId}/resources/${resourceId}/file`;
}
