import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Button,
  Grid,
} from "@mui/material";
import { useFormik } from "formik";
import * as Yup from "yup";
import { format } from "date-fns";

const validationSchema = Yup.object({
  name: Yup.string()
    .required("Name is required")
    .min(2, "Name must be at least 2 characters")
    .max(30, "Name mustn't exceed 30 characters"),
  mobilePhone: Yup.string()
    .required("Mobile phone is required")
    .matches(/^\+?[0-9]{10,15}$/, "Phone number isn't valid (10-15 digits)"),
  jobTitle: Yup.string()
    .required("Job title is required")
    .max(100, "Job title mustn't exceed 100 characters"),
  birthDate: Yup.date()
    .required("Birth date is required")
    .max(new Date(), "Birth date cannot be in the future"),
});

const ContactForm = ({
  isOpen,
  onClose,
  initialValues,
  onSubmit,
  isEdit,
  showSnackbar,
}) => {
  const defaultValues = {
    id: isEdit ? "" : undefined,
    name: "",
    mobilePhone: "",
    jobTitle: "",
    birthDate: format(new Date(), "yyyy-MM-dd"),
  };

  const normalizedInitialValues = {
    ...defaultValues,
    ...initialValues,
    birthDate: initialValues?.birthDate
      ? format(new Date(initialValues.birthDate), "yyyy-MM-dd")
      : defaultValues.birthDate,
  };

  const formik = useFormik({
    initialValues: normalizedInitialValues,
    enableReinitialize: true,
    validationSchema,
    onSubmit: async (values) => {
      try {
        const formattedValues = {
          ...values,
          birthDate: new Date(values.birthDate).toISOString(),
        };

        await onSubmit(formattedValues);
        onClose();
      } catch (error) {
        console.error("Error submitting form:", error);
        showSnackbar("Failed to submit form.", "error");
      }
    },
  });

  return (
    <Dialog open={isOpen} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>
        {isEdit ? "Edit Contact" : "Create New Contact"}
      </DialogTitle>
      <form onSubmit={formik.handleSubmit}>
        <DialogContent>
          <Grid container spacing={2}>
            <Grid>
              <TextField
                fullWidth
                id="name"
                name="name"
                label="Name"
                value={formik.values.name}
                onChange={formik.handleChange}
                error={formik.touched.name && Boolean(formik.errors.name)}
                helperText={formik.touched.name && formik.errors.name}
                margin="normal"
              />
            </Grid>
            <Grid>
              <TextField
                fullWidth
                id="mobilePhone"
                name="mobilePhone"
                label="Mobile Phone"
                value={formik.values.mobilePhone}
                onChange={formik.handleChange}
                error={
                  formik.touched.mobilePhone &&
                  Boolean(formik.errors.mobilePhone)
                }
                helperText={
                  formik.touched.mobilePhone && formik.errors.mobilePhone
                }
                margin="normal"
              />
            </Grid>
            <Grid>
              <TextField
                fullWidth
                id="jobTitle"
                name="jobTitle"
                label="Job Title"
                value={formik.values.jobTitle}
                onChange={formik.handleChange}
                error={
                  formik.touched.jobTitle && Boolean(formik.errors.jobTitle)
                }
                helperText={formik.touched.jobTitle && formik.errors.jobTitle}
                margin="normal"
              />
            </Grid>
            <Grid>
              <TextField
                fullWidth
                id="birthDate"
                name="birthDate"
                label="Birth Date"
                type="date"
                value={formik.values.birthDate}
                onChange={formik.handleChange}
                error={
                  formik.touched.birthDate && Boolean(formik.errors.birthDate)
                }
                helperText={formik.touched.birthDate && formik.errors.birthDate}
                margin="normal"
              />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={onClose}>Cancel</Button>
          <Button type="submit" color="primary" variant="contained">
            {isEdit ? "Update" : "Create"}
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
};

export default ContactForm;
