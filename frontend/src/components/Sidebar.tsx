import React from 'react';
import { Link } from 'react-router-dom';

const Sidebar: React.FC = () => {
  return (
    <div className="w-64 bg-gray-800 text-white p-5">
      <h1 className="text-2xl font-bold mb-10">API Manager</h1>
      <nav>
        <ul>
          <li className="mb-4">
            <Link to="/" className="hover:text-gray-300">Home</Link>
          </li>
          <li className="mb-4">
            <Link to="/dashboard" className="hover:text-gray-300">Dashboard</Link>
          </li>
          <li className="mb-4">
            <Link to="/apis" className="hover:text-gray-300">API List</Link>
          </li>
          <li className="mb-4">
            <Link to="/api-products" className="hover:text-gray-300">API Products</Link>
          </li>
          <li className="mb-4">
            <Link to="/apis/create" className="hover:text-gray-300">Create API</Link>
          </li>
          <li>
            <Link to="/api-products/create" className="hover:text-gray-300">Create API Product</Link>
          </li>
        </ul>
      </nav>
    </div>
  );
};

export default Sidebar;
