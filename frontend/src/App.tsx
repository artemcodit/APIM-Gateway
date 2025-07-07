import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Layout from './components/Layout';
import Dashboard from './pages/Dashboard';
import ApiList from './pages/ApiList';
import ApiCreate from './pages/ApiCreate';
import ApiEdit from './pages/ApiEdit';
import './App.css';

function App() {
  return (
    <Router>
      <Layout>
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/apis" element={<ApiList />} />
          <Route path="/apis/create" element={<ApiCreate />} />
          <Route path="/apis/edit/:id" element={<ApiEdit />} />
        </Routes>
      </Layout>
    </Router>
  );
}

export default App;
