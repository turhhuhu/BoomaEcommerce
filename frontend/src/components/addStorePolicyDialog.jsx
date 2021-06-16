import React, { Component } from "react";
import { connect } from "react-redux";
import Dialog from "@material-ui/core/Dialog";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";
import { Alert } from "@material-ui/lab";
import Select from "react-select";

const mapPolicyTypeToValueName = {
  ageRestriction: "minAge",
  maxCategoryAmount: "amount",
  maxProductAmount: "amount",
  maxTotalAmount: "amount",
  minTotalAmount: "amount",
  minCategoryAmount: "amount",
  minProductAmount: "amount",
};

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
  {
    value: "maxTotalAmount",
    label: "Max Total Amount",
    name: "type",
  },
  {
    value: "minTotalAmount",
    label: "Min Total Amount",
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
    productGuid: "",
    category: "",
    isMenuOpen: false,
  };

  handleChange = (event) => {
    if (event?.target) {
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

  handleOpenMenu = () => {
    this.setState({ isMenuOpen: true });
  };

  handleCloseMenu = () => {
    this.setState({ isMenuOpen: false });
  };

  dynamicStyle = () => {
    if (this.state.isMenuOpen) {
      if (this.props.isRoot) {
        return { height: "450px" };
      }
      return { height: "450px" };
    }
    return null;
  };

  handleSubmit = (event) => {
    event.preventDefault();
    if (!this.state.type) {
      this.setState({ error: "Policy type is required" });
      return;
    }
    if (
      (this.state.type === "composite" || this.state.type === "binary") &&
      !this.state.operator
    ) {
      this.setState({
        error: "Operator is needed with composite or binary policy type",
      });
      return;
    }
    if (
      this.state.type &&
      this.state.type !== "composite" &&
      this.state.type !== "binary" &&
      !this.state.value
    ) {
      this.setState({ error: "Value is needed with selected policy type" });
      return;
    }
    this.setState({ error: undefined });
    this.props.isRoot
      ? this.props
          .dispatch(
            this.props.addRootPolicy(this.props.storeGuid, {
              type: this.state.type,
              operator: this.state.operator ? this.state.operator : undefined,
              [mapPolicyTypeToValueName[this.state.type]]: this.state.value
                ? this.state.value
                : undefined,
              productGuid: this.state.productGuid
                ? this.state.productGuid
                : undefined,
              category: this.state.category ? this.state.category : undefined,
            })
          )
          .then((success) => {
            if (success) {
              this.props.closeDialog();
              this.props.dispatch(this.props.fetchPolicy(this.props.storeGuid));
            }
          })
      : this.props
          .dispatch(
            this.props.addSubPolicy(
              this.props.storeGuid,
              this.props.fatherPolicyGuid,
              {
                type: this.state.type,
                operator: this.state.operator ? this.state.operator : undefined,
                [mapPolicyTypeToValueName[this.state.type]]: this.state.value
                  ? this.state.value
                  : undefined,
                productGuid: this.state.productGuid
                  ? this.state.productGuid
                  : undefined,
                category: this.state.category ? this.state.category : undefined,
              }
            )
          )
          .then((success) => {
            if (success) {
              this.props.closeDialog();
              this.setState({
                error: undefined,
                type: "",
                operator: "",
                value: "",
                isTypeMenuOpen: false,
                isTypeOperatorMenuOpen: false,
              });
              this.props.dispatch(this.props.fetchPolicy(this.props.storeGuid));
            }
          });
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
          <DialogContent style={this.dynamicStyle()}>
            <DialogContentText>
              Please fill out the following policy details:
            </DialogContentText>
            <label>Type:</label>
            <Select
              onMenuOpen={this.handleOpenMenu}
              onMenuClose={this.handleCloseMenu}
              options={typeOptions}
              onChange={this.handleChange}
            />
            {(this.state.type && this.state.type === "ageRestriction") ||
            this.state.type === "maxProductAmount" ||
            this.state.type === "minProductAmount" ? (
              <React.Fragment>
                <br />
                <label>Product:</label>
                <Select
                  maxMenuHeight={200}
                  onMenuOpen={this.handleOpenMenu}
                  onMenuClose={this.handleCloseMenu}
                  options={this.props?.storeProducts.map((product) => {
                    return {
                      value: product.guid,
                      label: product.name,
                      name: "productGuid",
                    };
                  })}
                  onChange={this.handleChange}
                />{" "}
              </React.Fragment>
            ) : null}
            {(this.state.type && this.state.type === "maxCategoryAmount") ||
            this.state.type === "minCategoryAmount" ? (
              <React.Fragment>
                <br />
                <label>Category:</label>
                <Select
                  maxMenuHeight={200}
                  onMenuOpen={this.handleOpenMenu}
                  onMenuClose={this.handleCloseMenu}
                  options={[
                    ...new Set(
                      this.props.storeProducts?.map(
                        (product) => product.category
                      )
                    ),
                  ].map((category) => {
                    return {
                      value: category,
                      label: category,
                      name: "category",
                    };
                  })}
                  onChange={this.handleChange}
                />{" "}
              </React.Fragment>
            ) : null}
            {this.state.type === "composite" || this.state.type === "binary" ? (
              <React.Fragment>
                <br />
                <label>Operator:</label>
                <Select
                  onMenuOpen={this.handleOpenMenu}
                  onMenuClose={this.handleCloseMenu}
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
                  name="value"
                  required
                  value={this.state.value}
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

const mapStateToProps = (store) => {
  return {
    storeProducts: store.store.products,
    error: store.store.error,
  };
};

export default connect(mapStateToProps)(AddStorePolicyDialog);
