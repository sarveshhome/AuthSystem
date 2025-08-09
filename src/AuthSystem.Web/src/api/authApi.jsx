import axios from 'axios';

const API_URL = 'http://localhost:5062/api/auth';

export const login = async (email, password) => {
  const response = await axios.post(`${API_URL}/login`,
  JSON.stringify({ email, password }),
  {
      headers: { 'Content-Type': 'application/json' },
      withCredentials: true
  }
);
  console.log(JSON.stringify(response?.data));
  return response.data;
};

export const register = async (email, password) => {
  const response = await axios.post(`${API_URL}/register`, { email, password });
  return response.data;
};

export const refreshToken = async (token, refreshToken) => {
  const response = await axios.post(`${API_URL}/refresh-token`, { token, refreshToken });
  return response.data;
};