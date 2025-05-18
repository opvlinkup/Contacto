import React from 'react';
import {
  List,
  Typography,
  Paper,
  CircularProgress,
} from '@mui/material';
import ContactItem from '../components/ContactItem';
import { useContactsContext } from '../context/ContactsContext';
import contactsService from '../services/ContactsService';


const ContactList = () => {
  const { state, dispatch } = useContactsContext();
  const { contacts, loading, error } = state;

  const handleEditClick = (contact) => {
    dispatch({ type: 'SELECT_CONTACT', payload: contact });
    dispatch({ type: 'OPEN_EDIT_MODAL' });
  };

  const handleDeleteClick = async (id) => {
    try {
      console.log('Deleting contact with ID:', id);
      await contactsService.remove(id);
      dispatch({ type: 'DELETE_CONTACT', payload: id });
      dispatch({
        type: 'OPEN_SNACKBAR',
        payload: { message: 'Contact deleted successfully!' },
      });
    } catch (error) {
      console.error('Error deleting contact:', error);
    }
  };

  if (loading) {
    return (
      <Paper elevation={3} style={{ padding: '20px', textAlign: 'center' }}>
        <CircularProgress />
      </Paper>
    );
  }

  if (error) {
    return (
      <Paper elevation={3} style={{ padding: '20px', textAlign: 'center' }}>
        <Typography color="error">Error: {error.message}</Typography>
      </Paper>
    );
  }

  if (contacts.length === 0) {
    return (
      <Paper elevation={3} style={{ padding: '20px', textAlign: 'center' }}>
        <Typography>No contacts found. Create your first contact!</Typography>
      </Paper>
    );
  }

  return (  
      <List>
        {contacts.map((contact) => (
          <ContactItem
            key={contact.id}
            contact={contact}
            onEdit={handleEditClick}
            onDelete={handleDeleteClick}
          />
        ))}
      </List>
  );
};

export default ContactList;