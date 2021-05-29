import React, { Component } from "react";
import { connect } from "react-redux";
import TreeView from "@material-ui/lab/TreeView";
import ExpandMoreIcon from "@material-ui/icons/ExpandMore";
import ChevronRightIcon from "@material-ui/icons/ChevronRight";
import TreeItem from "@material-ui/lab/TreeItem";
import FunctionsIcon from "@material-ui/icons/Functions";
import { fetchStorePolicy, removeStorePolicy } from "../actions/storeActions";
import AddStorePolicyDialog from "./addStorePolicyDialog";

const mapPolicyTypeToLabel = {
  ageRestriction: "Age Restriction",
  maxCategoryAmount: "Max Category Amount",
  maxProductAmount: "Max Product Amount",
  maxTotalAmount: "Max Total Amount",
  minTotalAmount: "Max Total Amount",
  minCategoryAmount: "Min Category Amount",
  minProductAmount: "Min Product Amount",
};

class PolicyTree extends Component {
  state = {
    isDialogOpen: false,
    fatherPolicyGuid: undefined,
  };

  closeDialog = () => {
    this.setState({ isDialogOpen: false });
  };

  handleAddPolicy = (fatherPolicyGuid) => {
    this.setState({ isDialogOpen: true, fatherPolicyGuid: fatherPolicyGuid });
  };

  handleRemovePolicy = (policyGuid) => {
    this.props
      .dispatch(removeStorePolicy(this.props.storeGuid, policyGuid))
      .then(fetchStorePolicy(this.props.storeGuid));
  };

  createPolicyTree = (policy) => {
    if (policy.subPolicies) {
      return (
        <TreeItem key={policy.guid} label={policy.type} nodeId={policy.guid}>
          <button
            onClick={() => this.handleAddPolicy(policy.guid)}
            className="btn btn-outline-primary btn-sm mb-2 mt-2"
          >
            {" "}
            Add sub-policy
          </button>
          <button
            onClick={() => this.handleRemovePolicy(policy.guid)}
            className="btn btn-outline-secondary btn-sm mb-2 mt-2 ml-2"
          >
            {" "}
            Remove policy
          </button>
          <TreeItem
            icon={<FunctionsIcon />}
            label={policy.operator}
            nodeId={policy.guid + "1"}
            key={policy.guid + "1"}
          />
          {policy.subPolicies.map((subPolicy) =>
            this.createPolicyTree(subPolicy)
          )}
        </TreeItem>
      );
    } else {
      return (
        <TreeItem
          label={mapPolicyTypeToLabel[policy.type]}
          nodeId={policy.guid}
          key={policy.guid}
        >
          <button
            onClick={() => this.handleRemovePolicy(policy.guid)}
            className="btn btn-outline-secondary btn-sm mb-2 mt-2 ml-2"
          >
            {" "}
            Remove policy
          </button>
          <TreeItem
            label={
              policy[
                Object.keys(policy).find(
                  (prop) =>
                    prop === "minAge" ||
                    prop === "maxAmount" ||
                    prop === "minAmount"
                )
              ]
            }
            nodeId={policy.guid + "1"}
            key={policy.guid + "1"}
          />
        </TreeItem>
      );
    }
  };

  render() {
    return (
      <main className="col">
        {this.props.storePolicy?.type ? (
          <React.Fragment>
            <TreeView
              defaultCollapseIcon={<ExpandMoreIcon />}
              defaultExpandIcon={<ChevronRightIcon />}
            >
              {this.createPolicyTree(this.props.storePolicy)}
            </TreeView>
            <AddStorePolicyDialog
              fatherPolicyGuid={this.state.fatherPolicyGuid}
              storeGuid={this.props.storeGuid}
              isDialogOpen={this.state.isDialogOpen}
              closeDialog={this.closeDialog}
            />{" "}
          </React.Fragment>
        ) : null}
      </main>
    );
  }
}
const mapStateToProps = (store) => {
  return {
    storePolicy: store.store.storePolicy,
  };
};
export default connect(mapStateToProps)(PolicyTree);
