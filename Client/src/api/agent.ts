import axios, { AxiosError } from 'axios';

export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api';

const agent = axios.create({
  baseURL: API_BASE_URL,
});

const sleep = (delay: number) =>
  new Promise((resolve) => {
    setTimeout(resolve, delay);
  });

agent.interceptors.request.use((config) => {
  return config;
});

agent.interceptors.response.use(
  async (response) => {
    if (import.meta.env.DEV) {
      await sleep(300); // a bit shorter than 1s
    }
    return response;
  },
  (error: AxiosError) => {
    if (error.response) {
      console.error(
        `[API ERROR] ${error.config?.method?.toUpperCase() || 'GET'} ${error.config?.url} -> ${
          error.response.status
        }`,
        error.response.data
      );
    } else {
      console.error('[API ERROR] No response received', error.message);
    }

    return Promise.reject(error);
  }
);

export default agent;
