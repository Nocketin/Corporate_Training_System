import { api } from './axiosInstance';

export type EnrollmentStatus = 'Started' | 'Completed';

export interface CourseProgress {
  courseId: string;
  isEnrolled: boolean;
  status: EnrollmentStatus | null;
  totalLessons: number;
  completedLessons: number;
  progressPercent: number;
  completedLessonIds: string[];
  lockedLessonIds: string[];
  nextLessonId: string | null;
}

export interface CompleteLessonResult {
  lessonId: string;
  courseCompleted: boolean;
  enrollmentStatus: EnrollmentStatus;
  completedLessons: number;
  totalLessons: number;
}

export interface MyEnrollment {
  enrollmentId: string;
  courseId: string;
  courseTitle: string | null;
  status: EnrollmentStatus;
  startDate: string;
  completeDate: string | null;
  certificateFileUrl: string | null;
}

export async function fetchCourseProgress(courseId: string): Promise<CourseProgress> {
  const { data } = await api.get<CourseProgress>(`/api/learning/courses/${courseId}/progress`);
  return data;
}

export async function fetchMyEnrollments(): Promise<MyEnrollment[]> {
  const { data } = await api.get<MyEnrollment[]>('/api/learning/my-enrollments');
  return data;
}

export async function enrollInCourse(courseId: string): Promise<void> {
  await api.post(`/api/learning/enroll/${courseId}`);
}

export async function completeLesson(lessonId: string): Promise<CompleteLessonResult> {
  const { data } = await api.post<CompleteLessonResult>(`/api/learning/lessons/${lessonId}/complete`);
  return data;
}
