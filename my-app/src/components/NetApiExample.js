import React, { useState } from 'react';
import { useApi, useApiMutation } from '../hooks/useApi';
import ApiService from '../services/apiService';

const NetApiExample = () => {
  const [newUserName, setNewUserName] = useState('');
  
  // Fetch users from your .NET API
  const { data: users, loading, error, refetch } = useApi('/users');
  
  // Mutation hook for creating users
  const { mutate, loading: creating, error: createError } = useApiMutation();

  const handleCreateUser = async (e) => {
    e.preventDefault();
    if (!newUserName.trim()) return;

    try {
      await mutate(() => ApiService.createUser({ name: newUserName }));
      setNewUserName('');
      refetch(); // Refresh the users list
    } catch (err) {
      console.error('Failed to create user:', err);
    }
  };

  const handleDeleteUser = async (userId) => {
    try {
      await mutate(() => ApiService.deleteUser(userId));
      refetch(); // Refresh the users list
    } catch (err) {
      console.error('Failed to delete user:', err);
    }
  };

  if (loading) return <div>Loading users...</div>;
  if (error) return <div>Error loading users: {error}</div>;

  return (
    <div style={{ padding: '20px', maxWidth: '600px', margin: '0 auto' }}>
      <h2>Users from .NET API</h2>
      
      {/* Form to create new user */}
      <form onSubmit={handleCreateUser} style={{ marginBottom: '20px' }}>
        <div>
          <input
            type="text"
            value={newUserName}
            onChange={(e) => setNewUserName(e.target.value)}
            placeholder="Enter user name"
            style={{ padding: '8px', marginRight: '10px', width: '200px' }}
          />
          <button 
            type="submit" 
            disabled={creating}
            style={{ padding: '8px 16px' }}
          >
            {creating ? 'Creating...' : 'Add User'}
          </button>
        </div>
        {createError && <p style={{ color: 'red' }}>Error: {createError}</p>}
      </form>

      {/* Display users */}
      <div>
        <h3>Current Users:</h3>
        {users && users.length > 0 ? (
          <ul>
            {users.map((user) => (
              <li key={user.id} style={{ marginBottom: '10px', display: 'flex', alignItems: 'center' }}>
                <span style={{ marginRight: '10px' }}>
                  {user.name || user.username || `User ${user.id}`}
                </span>
                <button 
                  onClick={() => handleDeleteUser(user.id)}
                  style={{ 
                    padding: '4px 8px', 
                    backgroundColor: '#dc3545', 
                    color: 'white', 
                    border: 'none',
                    borderRadius: '4px',
                    cursor: 'pointer'
                  }}
                >
                  Delete
                </button>
              </li>
            ))}
          </ul>
        ) : (
          <p>No users found.</p>
        )}
      </div>

      <button 
        onClick={refetch}
        style={{ 
          padding: '8px 16px', 
          marginTop: '20px',
          backgroundColor: '#007bff',
          color: 'white',
          border: 'none',
          borderRadius: '4px',
          cursor: 'pointer'
        }}
      >
        Refresh Users
      </button>
    </div>
  );
};

export default NetApiExample;