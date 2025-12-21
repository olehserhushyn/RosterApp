// Get current week number
export function getCurrentWeekNumber(): number {
  const now = new Date();
  return getWeekNumber(now);
}

// Get week number for a date
export function getWeekNumber(date: Date): number {
  const d = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
  const dayNum = d.getUTCDay() || 7;
  d.setUTCDate(d.getUTCDate() + 4 - dayNum);
  const yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
  return Math.ceil(((d.getTime() - yearStart.getTime()) / 86400000 + 1) / 7);
}

// Get total weeks in a year
export function getWeeksInYear(year: number): number {
  const d = new Date(year, 11, 31);
  const week = getWeekNumber(d);
  return week === 1 ? 53 : week;
}
