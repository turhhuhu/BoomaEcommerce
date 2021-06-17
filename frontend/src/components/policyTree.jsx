import React, { Component } from "react";
import { connect } from "react-redux";
import TreeView from "@material-ui/lab/TreeView";
import ExpandMoreIcon from "@material-ui/icons/ExpandMore";
import ChevronRightIcon from "@material-ui/icons/ChevronRight";
import TreeItem from "@material-ui/lab/TreeItem";
import AddStorePolicyDialog from "./addStorePolicyDialog";
import CallSplitIcon from "@material-ui/icons/CallSplit";

const mapPolicyTypeToLabel = {
  ageRestriction: "Age Restriction",
  maxCategoryAmount: "Max Category Amount",
  maxProductAmount: "Max Product Amount",
  maxTotalAmount: "Max Total Amount",
  minTotalAmount: "Min Total Amount",
  minCategoryAmount: "Min Category Amount",
  minProductAmount: "Min Product Amount",
};

const mapOperatorToLabel = {
  or: "At least one of (Or operator):",
  xor: "Exactly one of (Xor operator):",
  condition: "Condition (Condition operator):",
  and: "All of the following (And operator):",
};

const mapPolicyTypeToValueName = {
  ageRestriction: "Min age",
  maxCategoryAmount: "Max Amount",
  maxProductAmount: "Max Amount",
  maxTotalAmount: "Max Amount",
  minTotalAmount: "Min Amount",
  minCategoryAmount: "Min Amount",
  minProductAmount: "Min Amount",
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
      .dispatch(this.props.removePolicy(this.props.storeGuid, policyGuid))
      .then(() => {
        this.props.dispatch(this.props.fetchPolicy(this.props.storeGuid));
      });
  };

  componentDidUpdate() {
    console.log("updated");
  }

  createPolicyTree = (policy) => {
    if (policy.type === "composite" || policy.type === "binary") {
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
            icon={<CallSplitIcon />}
            label={mapOperatorToLabel[policy.operator]}
            nodeId={policy.guid + "1"}
            key={policy.guid + "1"}
          />
          {policy.subPolicies
            ? policy.subPolicies.map((subPolicy) =>
                this.createPolicyTree(subPolicy)
              )
            : [policy.firstPolicy, policy.secondPolicy].map((subPolicy) =>
                subPolicy ? this.createPolicyTree(subPolicy) : null
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
          {policy.type === "ageRestriction" ||
          policy.type === "maxProductAmount" ||
          policy.type === "minProductAmount" ? (
            <TreeItem
              label={`Product name: 
                ${
                  this.props.storeProducts.find(
                    (product) => product.guid === policy.productGuid
                  )?.name
                }`}
              nodeId={policy.guid + "1"}
              key={policy.guid + "1"}
            ></TreeItem>
          ) : null}
          {policy.type === "maxCategoryAmount" ||
          policy.type === "minCategoryAmount" ? (
            <TreeItem
              label={`Category: 
                ${policy.category}`}
              nodeId={policy.guid + "1"}
              key={policy.guid + "1"}
            ></TreeItem>
          ) : null}
          <TreeItem
            label={`${mapPolicyTypeToValueName[policy.type]}: 
              ${
                policy[
                  Object.keys(policy).find(
                    (prop) => prop === "minAge" || prop === "amount"
                  )
                ]
              }
            `}
            nodeId={policy.guid + "2"}
            key={policy.guid + "2"}
          />
        </TreeItem>
      );
    }
  };

  render() {
    return (
      <main className="col">
        {this.props.policies?.type ? (
          <React.Fragment>
            <TreeView
              defaultCollapseIcon={<ExpandMoreIcon />}
              defaultExpandIcon={<ChevronRightIcon />}
            >
              {this.createPolicyTree(this.props.policies)}
            </TreeView>
            <AddStorePolicyDialog
              addRootPolicy={this.props.addRootPolicy}
              addSubPolicy={this.props.addSubPolicy}
              fetchPolicy={this.props.fetchPolicy}
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
    storeProducts: store.store.products,
  };
};
export default connect(mapStateToProps)(PolicyTree);
