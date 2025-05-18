import {
  Avatar,
  IconButton,
  Box,
  Typography,
  Stack,
  Tooltip,
  ListItem,
  ListItemAvatar,
} from "@mui/material";
import { Edit, Delete } from "@mui/icons-material";
import { format, parseISO } from "date-fns";

const ContactItem = ({ contact, onEdit, onDelete }) => {
  const { id, name, jobTitle, mobilePhone, birthDate } = contact;

  const formattedBirthDate = birthDate
    ? format(parseISO(birthDate), "dd MMM yyyy")
    : "—";

  const renderInfo = (label, value, color = "#aaa") =>
    value && (
      <Typography variant="body2" sx={{ color }} component="div">
        <strong>{label}:</strong> {value}
      </Typography>
    );

  return (
    <ListItem
      sx={{
        backgroundColor: "#222",
        borderRadius: 2,
        mb: 1,
        px: 2,
        py: 2,
        alignItems: "flex-start",
        boxShadow: 3,
      }}
      divider
      disableGutters
    >
      <ListItemAvatar>
        <Avatar sx={{ bgcolor: "#7e57c2", width: 48, height: 48 }}>
          {(name?.[0] ?? "?").toUpperCase()}
        </Avatar>
      </ListItemAvatar>

      <Box sx={{ flexGrow: 1, ml: 2 }}>
        {renderInfo("Name", name, "#fff")}
        {renderInfo("Job Title", jobTitle)}
        {renderInfo("Phone", mobilePhone)}
        {renderInfo("Birthday", formattedBirthDate)}
      </Box>

      <Stack direction="column" spacing={1} sx={{ ml: 2 }}>
        <Tooltip title="Edit">
          <IconButton onClick={() => onEdit(contact)} sx={{ color: "#7e57c2" }}>
            <Edit />
          </IconButton>
        </Tooltip>
        <Tooltip title="Delete">
          <IconButton
            onClick={() => onDelete(id)}
            sx={{ color: "secondary.main" }}
          >
            <Delete />
          </IconButton>
        </Tooltip>
      </Stack>
    </ListItem>
  );
};

export default ContactItem;
