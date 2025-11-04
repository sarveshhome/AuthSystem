// src/App.test.jsx
import { render, screen } from './test/test-utils';
import App from './App';

describe('App', () => {
  it('renders the application header', () => {
    render(<App />);
    
    // Check for the app title
    expect(screen.getByText('AuthSystem')).toBeInTheDocument();
    
    // Check for navigation links
    expect(screen.getByRole('link', { name: 'Login' })).toBeInTheDocument();
    expect(screen.getByRole('link', { name: 'Register' })).toBeInTheDocument();
  });

  it('renders the welcome message', () => {
    render(<App />);
    
    // Check for the welcome heading
    expect(screen.getByRole('heading', { 
      name: 'Welcome to AuthSystem' 
    })).toBeInTheDocument();
    
    // Check for the login/register message
    expect(screen.getByText('Please login or register to continue.')).toBeInTheDocument();
  });

  it('renders the navigation bar', () => {
    render(<App />);
    
    // Check for the navigation element
    expect(screen.getByRole('navigation')).toBeInTheDocument();
  });
});
