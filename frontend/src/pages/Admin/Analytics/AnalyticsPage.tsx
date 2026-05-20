import { useCallback, useEffect, useMemo, useState } from 'react';
import {
  Bar,
  BarChart,
  CartesianGrid,
  Legend,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts';
import {
  fetchLearningAnalyticsOverview,
  type LearningAnalyticsOverview,
} from '../../../api/analytics';
import { PageHeader } from '../../../components/PageHeader/PageHeader';
import { AnalyticsSkeleton } from './components/AnalyticsSkeleton';
import styles from './AnalyticsPage.module.scss';

type PeriodPreset = 7 | 30 | 90;

function formatDate(d: Date): string {
  return d.toISOString().slice(0, 10);
}

function buildRange(days: PeriodPreset): { from: string; to: string } {
  const to = new Date();
  const from = new Date();
  from.setUTCDate(from.getUTCDate() - days + 1);
  return { from: formatDate(from), to: formatDate(to) };
}

export const AnalyticsPage = () => {
  const [period, setPeriod] = useState<PeriodPreset>(30);
  const [data, setData] = useState<LearningAnalyticsOverview | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const range = useMemo(() => buildRange(period), [period]);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const overview = await fetchLearningAnalyticsOverview(range);
      setData(overview);
    } catch {
      setError('Не удалось загрузить аналитику.');
      setData(null);
    } finally {
      setLoading(false);
    }
  }, [range]);

  useEffect(() => {
    void load();
  }, [load]);

  const timeline = useMemo(() => {
    if (!data) return [];
    const map = new Map<string, { date: string; enrollments: number; completions: number }>();

    for (const item of data.enrollmentsByDay) {
      map.set(item.date, { date: item.date, enrollments: item.count, completions: 0 });
    }
    for (const item of data.completionsByDay) {
      const existing = map.get(item.date) ?? { date: item.date, enrollments: 0, completions: 0 };
      existing.completions = item.count;
      map.set(item.date, existing);
    }

    return Array.from(map.values()).sort((a, b) => a.date.localeCompare(b.date));
  }, [data]);

  if (loading) {
    return <AnalyticsSkeleton />;
  }

  if (error || !data) {
    return <p className={styles.error}>{error ?? 'Нет данных'}</p>;
  }

  const { summary } = data;

  return (
    <div className={styles.page}>
      <PageHeader
        title="Аналитика обучения"
        subtitle="Записи, завершения и активность по курсам"
      />

      <div className={styles.toolbar}>
        <span className={styles.periodLabel}>Период:</span>
        {([7, 30, 90] as PeriodPreset[]).map((days) => (
          <button
            key={days}
            type="button"
            className={period === days ? styles.periodActive : styles.periodBtn}
            onClick={() => setPeriod(days)}
          >
            {days} дн.
          </button>
        ))}
      </div>

      <div className={styles.kpiGrid}>
        <article className={styles.kpiCard}>
          <span className={styles.kpiValue}>{summary.totalEnrollments}</span>
          <span className={styles.kpiLabel}>Записей</span>
        </article>
        <article className={styles.kpiCard}>
          <span className={styles.kpiValue}>{summary.completedEnrollments}</span>
          <span className={styles.kpiLabel}>Завершено</span>
        </article>
        <article className={styles.kpiCard}>
          <span className={styles.kpiValue}>{summary.activeEnrollments}</span>
          <span className={styles.kpiLabel}>В процессе</span>
        </article>
        <article className={styles.kpiCard}>
          <span className={styles.kpiValue}>{summary.completionRatePercent}%</span>
          <span className={styles.kpiLabel}>Доля завершений</span>
        </article>
      </div>

      <div className={styles.chartCard}>
        <h3 className={styles.chartTitle}>Динамика записей и завершений</h3>
        <ResponsiveContainer width="100%" height={280}>
          <LineChart data={timeline}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="date" />
            <YAxis allowDecimals={false} />
            <Tooltip />
            <Legend />
            <Line type="monotone" dataKey="enrollments" name="Записи" stroke="#2563eb" strokeWidth={2} />
            <Line type="monotone" dataKey="completions" name="Завершения" stroke="#16a34a" strokeWidth={2} />
          </LineChart>
        </ResponsiveContainer>
      </div>

      <div className={styles.chartCard}>
        <h3 className={styles.chartTitle}>Топ курсов по записям</h3>
        <ResponsiveContainer width="100%" height={320}>
          <BarChart data={data.coursesActivity} layout="vertical" margin={{ left: 24 }}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis type="number" allowDecimals={false} />
            <YAxis type="category" dataKey="title" width={160} />
            <Tooltip />
            <Legend />
            <Bar dataKey="enrollments" name="Записи" fill="#2563eb" />
            <Bar dataKey="completions" name="Завершения" fill="#16a34a" />
          </BarChart>
        </ResponsiveContainer>
      </div>
    </div>
  );
};
