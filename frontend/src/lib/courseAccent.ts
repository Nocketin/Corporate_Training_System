export const COURSE_ACCENTS = ['indigo', 'violet', 'cyan', 'emerald', 'amber'] as const;

export type CourseAccent = (typeof COURSE_ACCENTS)[number];

export function getCourseAccent(id: string): CourseAccent {
  let hash = 0;
  for (let i = 0; i < id.length; i += 1) {
    hash = (hash + id.charCodeAt(i)) % COURSE_ACCENTS.length;
  }
  return COURSE_ACCENTS[hash];
}

export function countLessonsFromDetail(modules: { lessons: unknown[] }[]): number {
  return modules.reduce((sum, m) => sum + m.lessons.length, 0);
}
