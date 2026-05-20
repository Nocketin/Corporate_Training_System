import { api } from './axiosInstance';

export type DailyCount = {
  date: string;
  count: number;
};

export type LearningAnalyticsSummary = {
  totalEnrollments: number;
  completedEnrollments: number;
  activeEnrollments: number;
  completionRatePercent: number;
};

export type CourseActivity = {
  courseId: string;
  title: string;
  enrollments: number;
  completions: number;
};

export type LearningAnalyticsOverview = {
  summary: LearningAnalyticsSummary;
  enrollmentsByDay: DailyCount[];
  completionsByDay: DailyCount[];
  coursesActivity: CourseActivity[];
};

export async function fetchLearningAnalyticsOverview(params?: {
  from?: string;
  to?: string;
}): Promise<LearningAnalyticsOverview> {
  const { data } = await api.get<LearningAnalyticsOverview>('/api/learning/analytics/overview', {
    params,
  });
  return data;
}
