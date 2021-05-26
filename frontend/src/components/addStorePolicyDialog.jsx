import React, { Component } from "react";
import { connect } from "react-redux";
import Dialog from "@material-ui/core/Dialog";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";
import { Alert } from "@material-ui/lab";
import Select from "react-select";

const typeOptions = [
  { value: "composite", label: "Composite", name: "type" },
  { value: "binary", label: "Binary", name: "type" },
  {
    value: "ageRestriction",
    label: "Age Restriction",
    name: "type",
  },
  {
    value: "maxCategoryAmount",
    label: "Max Category Amount",
    name: "type",
  },
  {
    value: "minCategoryAmount",
    label: "Min Category Amount",
    name: "type",
  },
  {
    value: "maxProductAmount",
    label: "Max Product Amount",
    name: "type",
  },
  {
    value: "minProductAmount",
    label: "Min Product Amount",
    name: "type",
  },
];
const operatorOptions = [
  { value: "and", label: "And", name: "operator" },
  { value: "or", label: "Or", name: "operator" },
  { value: "xor", label: "Xor", name: "operator" },
  { value: "condition", label: "Condition", name: "operator" },
];
class AddStorePolicyDialog extends Component {
  state = {
    error: undefined,
    type: "",
    operator: "",
    value: "",
    isTypeMenuOpen: false,
    isTypeOperatorMenuOpen: false,
  };

  handleChange = (event) => {
    if (this.event?.target) {
      this.setState({
        [event.target.name]: event.target.value,
      });
    } else {
      this.setState({
        [event.name]: event.value,
      });
    }
  };

  handleClose = (event) => {
    event.preventDefault();
    this.props.closeDialog();
  };

  handleOpenTypeMenu = () => {
    this.setState({ isTypeMenuOpen: true });
  };

  handleCloseTypeMenu = () => {
    this.setState({ isTypeMenuOpen: false });
  };

  handleOpenOperatorMenu = () => {
    this.setState({ isTypeOperatorMenuOpen: true });
  };

  handleCloseOperatorMenu = () => {
    this.setState({ isTypeOperatorMenuOpen: false });
  };

  handleSubmit = (event) => {
    if (!this.state.type) {
      this.setState({ error: "Policy type is required" });
      return;
    } else if (
      (this.state.type === "composite" || this.state.type === "binary") &&
      !this.state.operator
    ) {
      this.setState({
        error: "Operator is needed with composite or binary policy type",
      });
    } else if (
      this.state.type &&
      this.state.type !== "composite" &&
      this.state.type !== "binary" &&
      !this.state.value
    ) {
      this.setState({ error: "Value is needed with selected policy type" });
    } else {
      event.preventDefault();
      this.setState({ error: undefined });
      //   this.props
      //     .dispatch(
      //       addStorePolicy({
      //         type: this.state.name,
      //         description: this.state.description,
      //       })
      //     )
      //     .then((success) => {
      //       if (success) {
      //         this.props.closeDialog();
      //         this.props.dispatch(fetchUserRoles());
      //       }
      //     });
    }
  };

  render() {
    return (
      <Dialog
        open={this.props.isDialogOpen}
        onClose={this.handleClose}
        aria-labelledby="form-dialog-title"
      >
        <DialogTitle id="form-dialog-title">Add product</DialogTitle>
        <form>
          <DialogContent
            style={
              this.state.isTypeMenuOpen || this.state.isTypeOperatorMenuOpen
                ? { height: "450px" }
                : null
            }
          >
            <DialogContentText>
              Please fill out the following policy details:
            </DialogContentText>
            <label>Type:</label>
            <Select
              onMenuOpen={this.handleOpenTypeMenu}
              onMenuClose={this.handleCloseTypeMenu}
              options={typeOptions}
              onChange={this.handleChange}
            />

            {this.state.type === "composite" || this.state.type === "binary" ? (
              <React.Fragment>
                <br />
                <label>Operator:</label>
                <Select
                  onMenuOpen={this.handleOpenOperatorMenu}
                  onMenuClose={this.handleCloseOperatorMenu}
                  options={operatorOptions}
                  onChange={this.handleChange}
                />{" "}
              </React.Fragment>
            ) : null}
            {this.state.type &&
            this.state.type !== "composite" &&
            this.state.type !== "binary" ? (
              <React.Fragment>
                <br />
                <label>Value:</label>
                <input
                  type="text"
                  className="form-control mb-2"
                  name="nominatedUserName"
                  required
                  value={this.state.nominatedUserName}
                  onChange={this.handleChange}
                ></input>
              </React.Fragment>
            ) : null}
          </DialogContent>
          <DialogActions>
            <button
              className="btn btn-outline-primary my-2"
              onClick={this.handleClose}
            >
              Cancel
            </button>
            <button
              className="btn btn-outline-primary my-2"
              onClick={this.handleSubmit}
            >
              Add
            </button>
          </DialogActions>
          {(this.props.error || this.state.error) && (
            <Alert severity="error" onClick={() => this.setState(null)}>
              {this.props.error || this.state.error}
            </Alert>
          )}
        </form>
      </Dialog>
    );
  }
}

export default connect()(AddStorePolicyDialog);
